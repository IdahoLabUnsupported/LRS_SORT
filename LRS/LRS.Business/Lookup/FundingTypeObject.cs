using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace LRS.Business
{
    public class FundingTypeObject
    {
        #region Properties

        public int FundingTypeId { get; set; }
        public string FundingType { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public FundingTypeObject() { }

        #endregion

        #region Repository

        private static IFundingTypeRepository repo => new FundingTypeRepository();

        #endregion

        #region Static Methods

        public static List<FundingTypeObject> GetFundingTypes() => repo.GetFundingTypes();

        internal static FundingTypeObject GetFundingType(int fundingTypeId) => repo.GetFundingType(fundingTypeId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveFundingType(this);
            MemoryCache.ClearFundingTypes();
        }

        public void Delete()
        {
            repo.DeleteFundingType(this);
            MemoryCache.ClearFundingTypes();
        }

        #endregion
    }

    public interface IFundingTypeRepository
    {
        List<FundingTypeObject> GetFundingTypes();
        FundingTypeObject GetFundingType(int fundingTypeId);
        FundingTypeObject SaveFundingType(FundingTypeObject fundingType);
        bool DeleteFundingType(FundingTypeObject fundingType);
    }

    public class FundingTypeRepository : IFundingTypeRepository
    {
        public List<FundingTypeObject> GetFundingTypes() => Config.Conn.Query<FundingTypeObject>("SELECT * FROM lu_FundingType").ToList();

        public FundingTypeObject GetFundingType(int fundingTypeId) => Config.Conn.Query<FundingTypeObject>("SELECT * FROM lu_FundingType WHERE FundingTypeId = @FundingTypeId", new { FundingTypeId = fundingTypeId }).FirstOrDefault();

        public FundingTypeObject SaveFundingType(FundingTypeObject fundingType)
        {
            if (fundingType.FundingTypeId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_FundingType
                    SET     FundingType = @FundingType,
                            Description = @Description,
                            Active = @Active
                    WHERE   FundingTypeId = @FundingTypeId";
                Config.Conn.Execute(sql, fundingType);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_FundingType (
                        FundingType,
                        Description,
                        Active
                    )
                    VALUES (
                        @FundingType,
                        @Description,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                fundingType.FundingTypeId = Config.Conn.Query<int>(sql, fundingType).Single();
            }
            return fundingType;
        }

        public bool DeleteFundingType(FundingTypeObject fundingType)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_FundingType WHERE FundingTypeId = @FundingTypeId", fundingType);
            }
            catch { return false; }
            return true;
        }
    }
}
