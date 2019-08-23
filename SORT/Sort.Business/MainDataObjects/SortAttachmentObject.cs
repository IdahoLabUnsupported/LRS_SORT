using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Sort.Business
{
    public class SortAttachmentObject
    {
        #region Properties

        public int SortAttachmentId { get; set; }
        public int SortMainId { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadEmployeeId { get; set; }
        public string FileName { get; set; }
        public string Note { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedByEmployeeId { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string AttachmentType { get; set; }
        public string Size { get; set; }

        #endregion

        #region Extended Properties
        public AttachmentTypeEnum AttachmentTypeEnum
        {
            get { return AttachmentType.ToEnum<AttachmentTypeEnum>(); }
            set { AttachmentType = value.ToString(); }
        }

        public string AttachmentTypeName => AttachmentTypeEnum.GetEnumDisplayName();
        public string AddedByUserName => EmployeeCache.GetName(UploadEmployeeId);
        public string DeletedByUserName => EmployeeCache.GetName(DeletedByEmployeeId);
        public string SortTitle { get; set; }
        #endregion

        #region File Properties

        public Stream Contents
        {
            get
            {
                FilePartInfo partsInfo = repo.GetFilePartInfo(SortAttachmentId);
                MemoryStream st = null;

                // Have to handle the old way the files where saved.
                if (partsInfo.NumberParts == 1 && partsInfo.Size == 0)
                {
                    try
                    {
                        st = new MemoryStream();
                        var data = repo.GetFile(SortAttachmentId);
                        if (data != null)
                        {
                            st.Write(data, 0, data.Length);
                        }
                    }
                    catch { return null; }
                }
                else
                {
                    try
                    {
                        st = new MemoryStream(partsInfo.Size);
                        for (int part = 1; part <= partsInfo.NumberParts; part++)
                        {
                            var data = repo.GetFilePartContent(SortAttachmentId, part);
                            if (data != null)
                            {
                                st.Write(data, 0, data.Length);
                            }
                        }
                    }
                    catch { return null; }
                }

                st.Seek(0, SeekOrigin.Begin);

                return st;
            }
        }

        #endregion

        #region Constructor

        public SortAttachmentObject() { }

        #endregion

        #region Repository

        private static ISortAttachmentRepository repo => new SortAttachmentRepository();

        #endregion

        #region Static Methods

        public static List<SortAttachmentObject> GetSortAttachments(int sortMainId) => repo.GetSortAttachments(sortMainId);

        public static SortAttachmentObject GetSortAttachment(int sortAttachmentId) => repo.GetSortAttachment(sortAttachmentId);

        public static SortAttachmentObject GetFinalDocAttachment(int sortMainId) => repo.GetFinalDocAttachment(sortMainId);

        public static SortAttachmentObject GetOstiAttachment(int sortMainId) => repo.GetOstiAttachment(sortMainId);

        public static SortAttachmentObject AddAttachment(int sortMainId, string fileName, string description, AttachmentTypeEnum attachmentType, int? size)
        {
            var sao = new SortAttachmentObject()
            {
                SortMainId = sortMainId,
                FileName = fileName,
                Note = description,
                AttachmentTypeEnum = attachmentType,
                IsDeleted = false,
                Size = CalcSize(size)
            };
            sao.Save();

            return sao;
        }

        //public static SortAttachmentObject AddAttachment(int sortMainId, string fileName, string description, AttachmentTypeEnum attachmentType, int? size, byte[] contents)
        //{
        //    var sao = AddAttachment(sortMainId, fileName, description, attachmentType, size);
        //    repo.UploadFile(sao.SortAttachmentId, contents);

        //    return sao;
        //}

        public static SortAttachmentObject AddOstiAttachment(int sortMainId, string fileName, string employeeId, int? size, byte[] contents)
        {
            // There can be only one, so check to see if there already is one. If so, mark it as deleted.
            GetOstiAttachment(sortMainId)?.PerminateDelete();
            // Now add in a new one.
            var sao = new SortAttachmentObject()
            {
                SortMainId = sortMainId,
                FileName = fileName,
                Note = "Transformed Final Document with cover sheet ready for OSTI",
                AttachmentType = "OstiDoc",
                UploadEmployeeId = employeeId,
                IsDeleted = false,
                Size = "Unknown"
            };
            sao.Save(false);
            repo.UploadFile(sao.SortAttachmentId, contents);

            return sao;
        }

        private static string CalcSize(int? size)
        {
            string sizeStr = string.Empty;
            if (size.HasValue)
            {
                double num = (double)size.Value / 1024.0;
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

            return sizeStr;
        }
        #endregion

        #region Object Methods

        public void Save(bool checkStatus = true)
        {
            if (SortAttachmentId == 0)
            {
                UploadDate = DateTime.Now;
                UploadEmployeeId = string.IsNullOrWhiteSpace(UploadEmployeeId) ? UserObject.CurrentUser.EmployeeId : UploadEmployeeId;
            }

            repo.SaveSortAttachment(this);
            if (checkStatus)
            {
                SortMainObject.CheckStatusUpdate(SortMainId, true);
            }
        }
        
        public void Delete()
        {
            IsDeleted = true;
            try
            {
                DeletedByEmployeeId = UserObject.CurrentUser.EmployeeId;
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("SortAttachmentObject::Delete", ex);
            }
            DeletedDate = DateTime.Now;
            Save();

            // If we mark the Final Document as deleted, we need to perminately delete the OSTI Attachment.
            // This way a new one will be generated.
            if (AttachmentTypeEnum == AttachmentTypeEnum.FinalDoc)
            {
                GetOstiAttachment(SortMainId)?.PerminateDelete();
            }
        }

        /// <summary>
        /// Perminatelly delete the document.  
        /// Only use if this is the OSTI Document. Otherwise use delete
        /// </summary>
        public void PerminateDelete()
        {
            if (AttachmentType == "OstiDoc")
            {
                repo.Delete(this);
            }
        }

        public void AddPart(int partNumber, byte[] contents) => repo.UploadFilePart(SortAttachmentId, partNumber, contents);

        #endregion
    }

    public interface ISortAttachmentRepository
    {
        List<SortAttachmentObject> GetSortAttachments(int sortMainId);
        SortAttachmentObject GetSortAttachment(int sortAttachmentId);
        SortAttachmentObject SaveSortAttachment(SortAttachmentObject sortAttachment);
        void UploadFile(int sortAttachmentId, byte[] contents);
        void UploadFilePart(int sortAttachmentId, int partNumber, byte[] contents);
        byte[] GetFile(int sortAttachmentId);
        SortAttachmentObject GetFinalDocAttachment(int sortMainId);
        SortAttachmentObject GetOstiAttachment(int sortMainId);
        bool Delete(SortAttachmentObject sortAttachment);
        FilePartInfo GetFilePartInfo(int sortAttachmentId);
        byte[] GetFilePartContent(int sortAttachmentId, int partNumber);
    }

    public class SortAttachmentRepository : ISortAttachmentRepository
    {
        public List<SortAttachmentObject> GetSortAttachments(int sortMainId)
        {
            string sql = @" SELECT a.*, s.Title as 'SortTitle'
                            FROM dat_SortAttachment a
                            INNER JOIN dat_SortMain s on s.SortMainId = a.SortMainId
                            WHERE a.SortMainId = @SortMainId 
                            AND a.IsDeleted = 0
                            AND a.AttachmentType != 'OstiDoc'";

            return Config.Conn.Query<SortAttachmentObject>(sql, new { SortMainId = sortMainId }).ToList();
        }

        public SortAttachmentObject GetSortAttachment(int sortAttachmentId)
        {
            string sql = @" SELECT a.*, s.Title as 'SortTitle'
                            FROM dat_SortAttachment a
                            INNER JOIN dat_SortMain s on s.SortMainId = a.SortMainId
                            WHERE a.SortAttachmentId = @SortAttachmentId ";

            return Config.Conn.Query<SortAttachmentObject>(sql, new { SortAttachmentId = sortAttachmentId }).FirstOrDefault();
        }

        public SortAttachmentObject GetFinalDocAttachment(int sortMainId)
        {
            string sql = @" SELECT a.*, s.Title as 'SortTitle'
                            FROM dat_SortAttachment a
                            INNER JOIN dat_SortMain s on s.SortMainId = a.SortMainId
                            WHERE a.SortMainId = @SortMainId 
                            AND a.AttachmentType = 'FinalDoc'
                            AND a.IsDeleted = 0";

            return Config.Conn.Query<SortAttachmentObject>(sql, new { SortMainId = sortMainId }).FirstOrDefault();
        }

        public SortAttachmentObject GetOstiAttachment(int sortMainId)
        {
            string sql = @" SELECT a.*, s.Title as 'SortTitle'
                            FROM dat_SortAttachment a
                            INNER JOIN dat_SortMain s on s.SortMainId = a.SortMainId
                            WHERE a.SortMainId = @SortMainId 
                            AND a.AttachmentType = 'OstiDoc'
                            AND a.IsDeleted = 0";

            return Config.Conn.Query<SortAttachmentObject>(sql, new { SortMainId = sortMainId }).FirstOrDefault();
        }

        public byte[] GetFile(int sortAttachmentId) => Config.Conn.Query<byte[]>("SELECT Contents FROM dat_SortAttachmentFile WHERE SortAttachmentId = @SortAttachmentId ", new { SortAttachmentId = sortAttachmentId }).FirstOrDefault();

        public FilePartInfo GetFilePartInfo(int sortAttachmentId) => Config.Conn.QueryFirstOrDefault<FilePartInfo>("select max(PartNumber) as NumberParts, SUM(Size) as Size FROM dat_SortAttachmentFile WHERE SortAttachmentId = @SortAttachmentId", new { SortAttachmentId = sortAttachmentId });

        public byte[] GetFilePartContent(int sortAttachmentId, int partNumber) => Config.Conn.QueryFirstOrDefault<byte[]>("SELECT Contents FROM dat_SortAttachmentFile WHERE SortAttachmentId = @SortAttachmentId AND PartNumber = @PartNumber ", new { SortAttachmentId = sortAttachmentId, PartNumber = partNumber });
        
        public SortAttachmentObject SaveSortAttachment(SortAttachmentObject sortAttachment)
        {
            if (sortAttachment.SortAttachmentId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_SortAttachment
                    SET     SortMainId = @SortMainId,
                            UploadDate = @UploadDate,
                            UploadEmployeeId = @UploadEmployeeId,
                            FileName = @FileName,
                            Note = @Note,
                            IsDeleted = @IsDeleted,
                            DeletedByEmployeeId = @DeletedByEmployeeId,
                            DeletedDate = @DeletedDate,
                            AttachmentType = @AttachmentType,
                            Size = @Size
                    WHERE   SortAttachmentId = @SortAttachmentId";
                Config.Conn.Execute(sql, sortAttachment);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_SortAttachment (
                        SortMainId,
                        UploadDate,
                        UploadEmployeeId,
                        FileName,
                        Note,
                        IsDeleted,
                        DeletedByEmployeeId,
                        DeletedDate,
                        AttachmentType,
                        Size
                    )
                    VALUES (
                        @SortMainId,
                        @UploadDate,
                        @UploadEmployeeId,
                        @FileName,
                        @Note,
                        @IsDeleted,
                        @DeletedByEmployeeId,
                        @DeletedDate,
                        @AttachmentType,
                        @Size
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                sortAttachment.SortAttachmentId = Config.Conn.Query<int>(sql, sortAttachment).Single();
            }
            return sortAttachment;
        }

        public void UploadFile(int sortAttachmentId, byte[] contents)
        {
            var sql = "INSERT INTO dat_SortAttachmentFile (SortAttachmentId, Contents) VALUES (@SortAttachmentId, @Contents)";

            var dParams = new DynamicParameters();
            dParams.Add("@SortAttachmentId", sortAttachmentId, DbType.Int32);
            dParams.Add("@Contents", contents, DbType.Binary);

            Config.Conn.Open();
            Config.Conn.Execute(sql, dParams);
            Config.Conn.Close();
        }

        public void UploadFilePart(int sortAttachmentId, int partNumber, byte[] contents)
        {
            var sql = "INSERT INTO dat_SortAttachmentFile (SortAttachmentId, PartNumber, Size, Contents) VALUES (@SortAttachmentId, @PartNumber, @Size, @Contents)";

            var dParams = new DynamicParameters();
            dParams.Add("@SortAttachmentId", sortAttachmentId, DbType.Int32);
            dParams.Add("@PartNumber", partNumber, DbType.Int32);
            dParams.Add("@Size", contents.Length, DbType.Int32);
            dParams.Add("@Contents", contents, DbType.Binary);

            Config.Conn.Open();
            Config.Conn.Execute(sql, dParams);
            Config.Conn.Close();
        }

        public bool Delete(SortAttachmentObject sortAttachment)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_SortAttachmentFile WHERE SortAttachmentId = @SortAttachmentId", sortAttachment);
                Config.Conn.Execute("DELETE FROM dat_SortAttachment WHERE SortAttachmentId = @SortAttachmentId", sortAttachment);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("SortAttachmentObject::Delete", ex);
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