using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sort.Business
{
    public class SponsorOrgObject
    {
        #region Properties

        public int SponsorOrgId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Constructor

        public SponsorOrgObject() { }

        #endregion

        #region Repository

        private static ISponsorOrgRepository repo => new SponsorOrgRepository();

        #endregion

        #region Static Methods

        public static List<SponsorOrgObject> GetSponsorOrgs() => repo.GetSponsorOrgs();

        public static SponsorOrgObject GetSponsorOrg(int sponsorOrgId) => repo.GetSponsorOrg(sponsorOrgId);

        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveSponsorOrg(this);
            MemoryCache.ClearSponsoringOrgs();
        }

        public bool Delete()
        {
            MemoryCache.ClearSponsoringOrgs();
            return repo.DeleteSponsorOrg(this);
        }

        #endregion
    }

    public interface ISponsorOrgRepository
    {
        List<SponsorOrgObject> GetSponsorOrgs();
        SponsorOrgObject GetSponsorOrg(int sponsorOrgId);
        SponsorOrgObject SaveSponsorOrg(SponsorOrgObject sponsorOrg);
        bool DeleteSponsorOrg(SponsorOrgObject sponsorOrg);
    }

    public class SponsorOrgRepository : ISponsorOrgRepository
    {
        public List<SponsorOrgObject> GetSponsorOrgs() => Config.Conn.Query<SponsorOrgObject>("SELECT * FROM lu_SponsorOrg").ToList();

        public SponsorOrgObject GetSponsorOrg(int sponsorOrgId) => Config.Conn.Query<SponsorOrgObject>("SELECT * FROM lu_SponsorOrg WHERE SponsorOrgId = @SponsorOrgId", new { SponsorOrgId = sponsorOrgId }).FirstOrDefault();

        public SponsorOrgObject SaveSponsorOrg(SponsorOrgObject sponsorOrg)
        {
            if (sponsorOrg.SponsorOrgId > 0) // Update
            {
                string sql = @"
                    UPDATE  lu_SponsorOrg
                    SET     Name = @Name,
                            Code = @Code,
                            Active = @Active
                    WHERE   SponsorOrgId = @SponsorOrgId";
                Config.Conn.Execute(sql, sponsorOrg);
            }
            else
            {
                string sql = @"
                    INSERT INTO lu_SponsorOrg (
                        Name,
                        Code,
                        Active
                    )
                    VALUES (
                        @Name,
                        @Code,
                        @Active
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                sponsorOrg.SponsorOrgId = Config.Conn.Query<int>(sql, sponsorOrg).Single();
            }
            return sponsorOrg;
        }

        public bool DeleteSponsorOrg(SponsorOrgObject sponsorOrg)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM lu_SponsorOrg WHERE SponsorOrgId = @SponsorOrgId", sponsorOrg);
            }
            catch { return false; }
            return true;
        }
    }
}
