using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Sort.Business
{
    public class SortMetaDataObject
    {
        #region Properties

        public int? MetaDataId { get; set; }
        public int SortMainId { get; set; }
        public string OstiId { get; set; }
        public string MetaDataType { get; set; }
        public string Data { get; set; }
        #endregion

        #region Extended Properties

        public MetaDataTypeEnum MetaDataTypeEnum
        {
            get { return MetaDataType.ToEnum<MetaDataTypeEnum>(); }
            set { MetaDataType = value.ToString(); }
        }
        
        public string SortTitle { get; set; }
        #endregion

        #region Constructor

        public SortMetaDataObject(){}

        public SortMetaDataObject(MetaDataTypeEnum metaDataType)
        {
            MetaDataTypeEnum = metaDataType;
        }

        #endregion

        #region Repository

        private static ISortMetaDataRepository repo => new SortMetaDataRepository();

        #endregion

        #region Static Methods

        public static List<SortMetaDataObject> GetSortMetaDatas(int sortMainId, MetaDataTypeEnum metaDataType) => repo.GetSortMetaDatas(sortMainId, metaDataType);

        public static SortMetaDataObject GetSortMetaData(int metaDataId) => repo.GetSortMetaData(metaDataId);

        public static void AddNew(MetaDataTypeEnum metaDataType, int sortMainId, string data)
        {
            SortMetaDataObject meta = new SortMetaDataObject();
            meta.MetaDataTypeEnum = metaDataType;
            meta.SortMainId = sortMainId;
            meta.Data = data;
            meta.Save();
        }
        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveSortMetaData(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        public void Delete()
        {
            repo.DeleteSortMetaData(this);
            SortMainObject.CheckStatusUpdate(SortMainId, true);
        }

        #endregion

    }

    public interface ISortMetaDataRepository
    {
        List<SortMetaDataObject> GetSortMetaDatas(int sortMainId, MetaDataTypeEnum metaDataType);
        SortMetaDataObject GetSortMetaData(int metaDataId);
        SortMetaDataObject SaveSortMetaData(SortMetaDataObject sortMetaData);
        bool DeleteSortMetaData(SortMetaDataObject sortMetaData);
    }

    public class SortMetaDataRepository : ISortMetaDataRepository
    {
        public List<SortMetaDataObject> GetSortMetaDatas(int sortMainId, MetaDataTypeEnum metaDataType)
        {
            string sql = @" SELECT d.*, s.Title as 'SortTitle' 
                            FROM dat_SortMetaData d
                            inner join dat_SortMain s on s.SortMainId = d.SortMainId
                            WHERE d.SortMainId = @SortMainId 
                            AND d.MetaDataType = @MetaDataType";

            return Config.Conn.Query<SortMetaDataObject>(sql, new { SortMainId = sortMainId, MetaDataType = metaDataType.ToString() }).ToList();
        }

        public SortMetaDataObject GetSortMetaData(int metaDataId)
        {
            string sql = @" SELECT d.*, s.Title as 'SortTitle' 
                            FROM dat_SortMetaData d
                            inner join dat_SortMain s on s.SortMainId = d.SortMainId
                            WHERE d.MetaDataId = @MetaDataId";

            return Config.Conn.Query<SortMetaDataObject>(sql, new { MetaDataId = metaDataId }).FirstOrDefault();
        }

        public SortMetaDataObject SaveSortMetaData(SortMetaDataObject sortMetaData)
        {
            if (sortMetaData.MetaDataId.HasValue ) // Update
            {
                string sql = @"
                    UPDATE  dat_SortMetaData
                    SET     SortMainId = @SortMainId,
                            MetaDataType = @MetaDataType,
                            Data = @Data
                    WHERE   MetaDataId = @MetaDataId";
                Config.Conn.Execute(sql, sortMetaData);
            }
            else
            {
                string sql = @"
                    INSERT INTO dat_SortMetaData (
                        SortMainId,
                        MetaDataType,
                        Data
                    )
                    VALUES (
                        @SortMainId,
                        @MetaDataType,
                        @Data
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                sortMetaData.MetaDataId = Config.Conn.Query<int>(sql, sortMetaData).Single();
            }
            return sortMetaData;
        }

        public bool DeleteSortMetaData(SortMetaDataObject sortMetaData)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_SortMetaData WHERE MetaDataId = @MetaDataId", sortMetaData);
            }
            catch (Exception ex)
            {
                ErrorLogObject.LogError("SortMetaDataObject::DeleteSortMetaData", ex);
                return false;
            }
            return true;
        }
    }
}
