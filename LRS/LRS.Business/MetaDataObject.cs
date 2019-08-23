using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class MetaDataObject
    {
        #region Properties

        public int MetaDataId { get; set; }
        public int MainId { get; set; }
        public string MetaDataType { get; set; }
        public string Data { get; set; }

        #endregion

        #region Extended Properties

        public MetaDataTypeEnum MetaDataTypeEnum
        {
            get => MetaDataType.ToEnum<MetaDataTypeEnum>();
            set => MetaDataType = value.ToString();
        }

        public string MainTitle => MainObject.GetMain(MainId)?.DisplayTitle;
        #endregion

        #region Constructor

        public MetaDataObject() { }

        #endregion

        #region Repository

        private static IMetaDataRepository repo => new MetaDataRepository();

        #endregion

        #region Static Methods

        public static List<MetaDataObject> GetMetaDatas(int mainId, MetaDataTypeEnum metaDataType) => repo.GetMetaDatas(mainId, metaDataType);

        public static MetaDataObject GetMetaData(int metaDataId) => repo.GetMetaData(metaDataId);

        public static void AddNew(MetaDataTypeEnum metaDataType, int mainId, string data)
        {
            MetaDataObject meta = new MetaDataObject();
            meta.MetaDataTypeEnum = metaDataType;
            meta.MainId = mainId;
            meta.Data = data;
            meta.Save();
        }

        public static bool CopyData(int fromMainId, int toMainId) => repo.CopyData(fromMainId, toMainId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveMetaData(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        public void Delete()
        {
            repo.DeleteMetaData(this);
            MainObject.UpdateActivityDateToNow(MainId);
        }

        #endregion
    }

    public interface IMetaDataRepository
    {
        List<MetaDataObject> GetMetaDatas(int mainId, MetaDataTypeEnum metaDataType);
        MetaDataObject GetMetaData(int metaDataId);
        MetaDataObject SaveMetaData(MetaDataObject metaData);
        bool DeleteMetaData(MetaDataObject metaData);
        bool CopyData(int fromMainId, int toMainId);
    }

    public class MetaDataRepository : IMetaDataRepository
    {
        public List<MetaDataObject> GetMetaDatas(int mainId, MetaDataTypeEnum metaDataType)
        {
            string sql = @" SELECT d.*, s.Title
                            FROM dat_MetaData d
                            inner join dat_Main s on s.MainId = d.MainId
                            WHERE d.MainId = @MainId 
                            AND d.MetaDataType = @MetaDataType";

            return Config.Conn.Query<MetaDataObject>(sql, new { MainId = mainId, MetaDataType = metaDataType.ToString() }).ToList();
        }

        public MetaDataObject GetMetaData(int metaDataId) => Config.Conn.Query<MetaDataObject>("SELECT * FROM dat_MetaData WHERE MetaDataId = @MetaDataId", new { MetaDataId = metaDataId }).FirstOrDefault();

        public MetaDataObject SaveMetaData(MetaDataObject metaData)
        {
            if (metaData.MetaDataId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_MetaData
                    SET     MainId = @MainId,
                            MetaDataType = @MetaDataType,
                            Data = @Data
                    WHERE   MetaDataId = @MetaDataId";
                Config.Conn.Execute(sql, metaData);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_MetaData (
                        MainId,
                        MetaDataType,
                        Data
                    )
                    VALUES (
                        @MainId,
                        @MetaDataType,
                        @Data
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                metaData.MetaDataId = Config.Conn.Query<int>(sql, metaData).Single();
            }
            return metaData;
        }

        public bool DeleteMetaData(MetaDataObject metaData)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_MetaData WHERE MetaDataId = @MetaDataId", metaData);
            }
            catch { return false; }
            return true;
        }

        public bool CopyData(int fromMainId, int toMainId)
        {
            string sql = @" insert into dat_MetaData (MainId, MetaDataType, [Data])
                            select	@NewMainId, MetaDataType, [Data]
                            FROM dat_MetaData
                            WHERE MainId = @OldMainId";

            try
            {
                Config.Conn.Execute(sql, new { NewMainId = toMainId, OldMainId = fromMainId });
            }
            catch { return false; }
            return true;
        }
    }
}
