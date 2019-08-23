using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sort.Business
{
    public class FundingTypeObject
    {
        #region Properties

        public int FundingTypeId { get; set; }
        [Required]
        public string FundingType { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
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

        public static FundingTypeObject GetFundingType(int fundingTypeId) => repo.GetFundingType(fundingTypeId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveFundingType(this);
            MemoryCache.ClearFundingTypes();
        }

        public bool Delete()
        {
            MemoryCache.ClearFundingTypes();
            return repo.DeleteFundingType(this);
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
