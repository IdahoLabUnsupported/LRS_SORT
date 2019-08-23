using Dapper;
using System.Collections.Generic;
using System.Linq;
using Inl.MvcHelper;

namespace LRS.Business
{
    public class GenericReviewDataObject
    {
        #region Properties

        public int GenericReviewDataId { get; set; }
        public string GenericEmail { get; set; }
        public string ReviewerType { get; set; }

        #endregion

        #region Extended Properties

        public ReviewerTypeEnum ReviewerTypeEnum
        {
            get => ReviewerType.ToEnum<ReviewerTypeEnum>();
            set => ReviewerType = value.ToString();
        }

        #endregion

        #region Constructor

        public GenericReviewDataObject() { }

        #endregion

        #region Repository

        private static IGenericReviewDataRepository repo => new GenericReviewDataRepository();

        #endregion

        #region Static Methods

        internal static List<GenericReviewDataObject> GetGenericReviewDatas() => repo.GetGenericReviewDatas();

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveGenericReviewData(this);
            MemoryCache.ClearGenericReviewData();
        }

        public void Delete()
        {
            repo.DeleteGenericReviewData(this);
            MemoryCache.ClearGenericReviewData();
        }

        #endregion
    }

    public interface IGenericReviewDataRepository
    {
        List<GenericReviewDataObject> GetGenericReviewDatas();
        GenericReviewDataObject GetGenericReviewData(int genericReviewDataId);
        GenericReviewDataObject SaveGenericReviewData(GenericReviewDataObject genericReviewData);
        bool DeleteGenericReviewData(GenericReviewDataObject genericReviewData);
    }

    public class GenericReviewDataRepository : IGenericReviewDataRepository
    {
        public List<GenericReviewDataObject> GetGenericReviewDatas() => Config.Conn.Query<GenericReviewDataObject>("SELECT * FROM lu_GenericReviewData").ToList();

        public GenericReviewDataObject GetGenericReviewData(int genericReviewDataId) => Config.Conn.Query<GenericReviewDataObject>("SELECT * FROM lu_GenericReviewData WHERE GenericReviewDataId = @GenericReviewDataId", new { GenericReviewDataId = genericReviewDataId }).FirstOrDefault();

        public GenericReviewDataObject SaveGenericReviewData(GenericReviewDataObject genericReviewData)
        {
            if (genericReviewData.GenericReviewDataId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_GenericReviewData
                    SET     GenericEmail = @GenericEmail,
                            ReviewerType = @ReviewerType
                    WHERE   GenericReviewDataId = @GenericReviewDataId";
                Config.Conn.Execute(sql, genericReviewData);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_GenericReviewData (
                        GenericEmail,
                        ReviewerType
                    )
                    VALUES (
                        @GenericEmail,
                        @ReviewerType
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                genericReviewData.GenericReviewDataId = Config.Conn.Query<int>(sql, genericReviewData).Single();
            }
            return genericReviewData;
        }

        public bool DeleteGenericReviewData(GenericReviewDataObject genericReviewData)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_GenericReviewData WHERE GenericReviewDataId = @GenericReviewDataId", genericReviewData);
            }
            catch { return false; }
            return true;
        }
    }
}
