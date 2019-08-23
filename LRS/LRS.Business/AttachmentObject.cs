using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;

namespace LRS.Business
{
    public class AttachmentObject
    {
        private const string ExternalFileLocation = @"\\webfiles\wwwroot\stm\";

        #region Properties

        public int AttachmentId { get; set; }
        public int MainId { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadEmployeeId { get; set; }
        public string FileName { get; set; }
        public string Note { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedByEmployeeId { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string Size { get; set; }
        public bool ExternalFile { get; set; }
        public int? NumberPages { get; set; }
        public bool AdminOnly { get; set; }
        #endregion

        #region Extended Properties
        public string AddedByUserName => EmployeeCache.GetFullName(UploadEmployeeId);
        public string DeletedByUserName => EmployeeCache.GetFullName(DeletedByEmployeeId);
        public string Title { get; set; }
        #endregion

        #region File Properties

        public Stream Contents
        {
            get
            {
                MemoryStream st = null;

                if (ExternalFile)
                {
                    string finalFileName = System.IO.Path.Combine(ExternalFileLocation, FileName);
                    try
                    {
                        if (File.Exists(finalFileName))
                        {
                            var data = System.IO.File.ReadAllBytes(finalFileName);
                            st = new MemoryStream(data);
                        }
                    }
                    catch { }
                }
                else
                {
                    FilePartInfo partsInfo = repo.GetFilePartInfo(AttachmentId);
                    st = new MemoryStream(partsInfo.Size);

                    try
                    {
                        for (int part = 1; part <= partsInfo.NumberParts; part++)
                        {
                            var data = repo.GetFilePartContent(AttachmentId, part);
                            if (data != null)
                            {
                                st.Write(data, 0, data.Length);
                            }
                        }
                    }
                    catch { return null; }
                }

                if (st != null)
                {
                    st.Seek(0, SeekOrigin.Begin);
                }

                return st;
            }
        }

        #endregion

        #region Constructor

        public AttachmentObject() { }

        #endregion

        #region Repository

        private static IAttachmentRepository repo => new AttachmentRepository();

        #endregion

        #region Static Methods

        public static List<AttachmentObject> GetAttachments(int mainId, bool adminOnly = false) => repo.GetAttachments(mainId, adminOnly);

        public static AttachmentObject GetAttachment(int attachmentId) => repo.GetAttachment(attachmentId);

        public static AttachmentObject AddAttachment(int mainId, string fileName, string description, int? size, int? numberPages, bool adminOnly = false, string employeeId = null)
        {
            string sizeStr = string.Empty;
            if (size.HasValue)
            {
                double num = (double) size.Value / 1024.0;
                if (num > 999999.99) //GigaBytes
                {
                    sizeStr = $"{(num / 1000000.0):F3} GB";
                }
                else if (num > 999.99) //MegaBytes
                {
                    sizeStr = $"{(num / 1000.0):F2} MB";
                }
                else //KiloBytes
                {
                    sizeStr = $"{num:F2} KB";
                }
            }

            var sao = new AttachmentObject()
            {
                MainId = mainId,
                FileName = fileName,
                Note = description,
                IsDeleted = false,
                Size = sizeStr,
                NumberPages =  numberPages,
                UploadEmployeeId = employeeId,
                AdminOnly = adminOnly
            };
            sao.Save();
            MainObject.UpdateActivityDateToNow(mainId);

            return sao;
        }

        public static AttachmentObject AddAttachment(int mainId, string fileName, string description, int? size, int? numberPages, byte[] contents, bool adminOnly = false)
        {
            var sao = AddAttachment(mainId, fileName, description, size, numberPages, adminOnly);
            repo.UploadFile(sao.AttachmentId, contents);
            MainObject.UpdateActivityDateToNow(mainId);

            return sao;
        }

        public static AttachmentObject AddAttachment(int mainId, string fileName, string description, int? size, int? numberPages, string employeeId, byte[] contents, bool adminOnly = false)
        {
            var sao = AddAttachment(mainId, fileName, description, size, numberPages, adminOnly, employeeId );
            repo.UploadFile(sao.AttachmentId, contents);
            MainObject.UpdateActivityDateToNow(mainId);

            return sao;
        }

        #endregion

        #region Object Methods

        public void Save()
        {
            if (AttachmentId == 0)
            {
                UploadDate = DateTime.Now;
                UploadEmployeeId = string.IsNullOrWhiteSpace(UploadEmployeeId) ? UserObject.CurrentUser.EmployeeId : UploadEmployeeId;
            }

            repo.SaveAttachment(this);
        }
        
        public void Delete()
        {
            IsDeleted = true;
            try
            {
                DeletedByEmployeeId = UserObject.CurrentUser.EmployeeId;
            }
            catch{}
            DeletedDate = DateTime.Now;
            Save();
        }

        public void AddPart(int partNumber, byte[] contents) => repo.UploadFilePart(AttachmentId, partNumber, contents);
        
        #endregion
    }

    public interface IAttachmentRepository
    {
        List<AttachmentObject> GetAttachments(int mainId, bool adminOnly = false);
        AttachmentObject GetAttachment(int attachmentId);
        AttachmentObject SaveAttachment(AttachmentObject attachment);
        void UploadFile(int attachmentId, byte[] contents);
        void UploadFilePart(int attachmentId, int partNumber, byte[] contents);
        byte[] GetFile(int attachmentId);
        FilePartInfo GetFilePartInfo(int attachmentId);
        byte[] GetFilePartContent(int attachmentId, int partNumber);
        bool Delete(AttachmentObject attachment);
    }

    public class AttachmentRepository : IAttachmentRepository
    {
        public List<AttachmentObject> GetAttachments(int mainId, bool adminOnly)
        {
            string sql = @" SELECT a.*, s.Title
                            FROM dat_Attachment a
                            INNER JOIN dat_Main s on s.MainId = a.MainId
                            WHERE a.MainId = @MainId 
                            AND a.AdminOnly = @AdminOnly
                            AND a.IsDeleted = 0";

            return Config.Conn.Query<AttachmentObject>(sql, new { MainId = mainId, AdminOnly = adminOnly }).ToList();
        }

        public AttachmentObject GetAttachment(int attachmentId)
        {
            string sql = @" SELECT a.*, s.Title
                            FROM dat_Attachment a
                            INNER JOIN dat_Main s on s.MainId = a.MainId
                            WHERE a.AttachmentId = @AttachmentId ";

            return Config.Conn.Query<AttachmentObject>(sql, new { AttachmentId = attachmentId }).FirstOrDefault();
        }

        public byte[] GetFile(int attachmentId) => Config.Conn.QueryFirstOrDefault<byte[]>("SELECT Contents FROM dat_AttachmentFile WHERE AttachmentId = @AttachmentId ", new { AttachmentId = attachmentId });

        public FilePartInfo GetFilePartInfo(int attachmentId) => Config.Conn.QueryFirstOrDefault<FilePartInfo>("select max(PartNumber) as NumberParts, SUM(Size) as Size FROM dat_AttachmentFile WHERE AttachmentId = @AttachmentId", new {AttachmentId = attachmentId});

        public byte[] GetFilePartContent(int attachmentId, int partNumber) => Config.Conn.QueryFirstOrDefault<byte[]>("SELECT Contents FROM dat_AttachmentFile WHERE AttachmentId = @AttachmentId AND PartNumber = @PartNumber ", new { AttachmentId = attachmentId, PartNumber = partNumber });

        public AttachmentObject SaveAttachment(AttachmentObject attachment)
        {
            if (attachment.AttachmentId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_Attachment
                    SET     IsDeleted = @IsDeleted,
                            DeletedByEmployeeId = @DeletedByEmployeeId,
                            DeletedDate = @DeletedDate                            
                    WHERE   AttachmentId = @AttachmentId";
                Config.Conn.Execute(sql, attachment);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_Attachment (
                        MainId,
                        UploadDate,
                        UploadEmployeeId,
                        FileName,
                        Note,
                        IsDeleted,
                        DeletedByEmployeeId,
                        DeletedDate,
                        Size,
                        NumberPages,
                        AdminOnly
                    )
                    VALUES (
                        @MainId,
                        @UploadDate,
                        @UploadEmployeeId,
                        @FileName,
                        @Note,
                        @IsDeleted,
                        @DeletedByEmployeeId,
                        @DeletedDate,
                        @Size,
                        @NumberPages,
                        @AdminOnly
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                attachment.AttachmentId = Config.Conn.Query<int>(sql, attachment).Single();
            }
            return attachment;
        }

        public void UploadFile(int attachmentId, byte[] contents)
        {
            var sql = "INSERT INTO dat_AttachmentFile (AttachmentId, PartNumber, Size, Contents) VALUES (@AttachmentId, @PartNumber, @Size, @Contents)";

            var dParams = new DynamicParameters();
            dParams.Add("@AttachmentId", attachmentId, DbType.Int32);
            dParams.Add("@PartNumber", 1, DbType.Int32);
            dParams.Add("@Size", contents.Length, DbType.Int32);
            dParams.Add("@Contents", contents, DbType.Binary);

            Config.Conn.Open();
            Config.Conn.Execute(sql, dParams);
            Config.Conn.Close();
        }

        public void UploadFilePart(int attachmentId, int partNumber, byte[] contents)
        {
            var sql = "INSERT INTO dat_AttachmentFile (AttachmentId, PartNumber, Size, Contents) VALUES (@AttachmentId, @PartNumber, @Size, @Contents)";

            var dParams = new DynamicParameters();
            dParams.Add("@AttachmentId", attachmentId, DbType.Int32);
            dParams.Add("@PartNumber", partNumber, DbType.Int32);
            dParams.Add("@Size", contents.Length, DbType.Int32);
            dParams.Add("@Contents", contents, DbType.Binary);

            Config.Conn.Open();
            Config.Conn.Execute(sql, dParams);
            Config.Conn.Close();
        }

        public bool Delete(AttachmentObject attachment)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_AttachmentFile WHERE AttachmentId = @AttachmentId", attachment);
                Config.Conn.Execute("DELETE FROM dat_Attachment WHERE AttachmentId = @AttachmentId", attachment);
            }
            catch 
            {
                return false;
            }
            return true;
        }
    }

    public class FilePartInfo
    {
        public int NumberParts { get; set; }
        public int Size { get; set; }
    }
}
