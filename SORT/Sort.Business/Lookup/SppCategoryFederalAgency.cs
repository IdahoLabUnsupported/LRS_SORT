using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sort.Business
{
    public class SppCategoryFederalAgencyObject
    {
        #region Properties

        public int SppCategoryFederalAgencyId { get; set; }
        [Required]
        public string FederalAgency { get; set; }
        [Required]
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public SppCategoryFederalAgencyObject() { }

        #endregion

        #region Repository

        private static ISppCategoryFederalAgencyRepository repo => new SppCategoryFederalAgencyRepository();

        #endregion

        #region Static Methods

        public static List<SppCategoryFederalAgencyObject> GetSppCategoryFederalAgencies() => repo.GetSppCategoryFederalAgencies();

        public static SppCategoryFederalAgencyObject GetSppCategoryFederalAgency(int sppCategoryFederalAgencyId) => repo.GetSppCategoryFederalAgency(sppCategoryFederalAgencyId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveSppCategoryFederalAgency(this);
            MemoryCache.ClearSppFundingFederalAgencies();
        }

        public bool Delete()
        {
            MemoryCache.ClearSppFundingFederalAgencies();
            return repo.DeleteSppCategoryFederalAgency(this);
        }

        #endregion
    }

    public interface ISppCategoryFederalAgencyRepository
    {
        List<SppCategoryFederalAgencyObject> GetSppCategoryFederalAgencies();
        SppCategoryFederalAgencyObject GetSppCategoryFederalAgency(int sppCategoryFederalAgencyId);
        SppCategoryFederalAgencyObject SaveSppCategoryFederalAgency(SppCategoryFederalAgencyObject sppCategoryFederalAgency);
        bool DeleteSppCategoryFederalAgency(SppCategoryFederalAgencyObject sppCategoryFederalAgency);
    }

    public class SppCategoryFederalAgencyRepository : ISppCategoryFederalAgencyRepository
    {
        public List<SppCategoryFederalAgencyObject> GetSppCategoryFederalAgencies() => Config.Conn.Query<SppCategoryFederalAgencyObject>("SELECT * FROM lu_SppCategoryFederalAgency").ToList();

        public SppCategoryFederalAgencyObject GetSppCategoryFederalAgency(int sppCategoryFederalAgencyId) => Config.Conn.Query<SppCategoryFederalAgencyObject>("SELECT * FROM lu_SppCategoryFederalAgency WHERE SppCategoryFederalAgencyId = @SppCategoryFederalAgencyId", new { SppCategoryFederalAgencyId = sppCategoryFederalAgencyId }).FirstOrDefault();

        public SppCategoryFederalAgencyObject SaveSppCategoryFederalAgency(SppCategoryFederalAgencyObject sppCategoryFederalAgency)
        {
            if (sppCategoryFederalAgency.SppCategoryFederalAgencyId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_SppCategoryFederalAgency
                    SET     FederalAgency = @FederalAgency,
                            Active = @Active
                    WHERE   SppCategoryFederalAgencyId = @SppCategoryFederalAgencyId";
                Config.Conn.Execute(sql, sppCategoryFederalAgency);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_SppCategoryFederalAgency (
                        FederalAgency,
                        Active
                    )
                    VALUES (
                        @FederalAgency,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                sppCategoryFederalAgency.SppCategoryFederalAgencyId = Config.Conn.Query<int>(sql, sppCategoryFederalAgency).Single();
            }
            return sppCategoryFederalAgency;
        }

        public bool DeleteSppCategoryFederalAgency(SppCategoryFederalAgencyObject sppCategoryFederalAgency)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_SppCategoryFederalAgency WHERE SppCategoryFederalAgencyId = @SppCategoryFederalAgencyId", sppCategoryFederalAgency);
            }
            catch { return false; }
            return true;
        }
    }
}