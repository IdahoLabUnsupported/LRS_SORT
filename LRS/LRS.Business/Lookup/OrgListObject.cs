using System.Collections.Generic;
using System.Linq;
using Dapper;


namespace LRS.Business
{
    public class OrgListObject
    {
        #region Properties

        public int EmployerCode { get; set; }
        public string Org { get; set; }
        public string OrgDesc { get; set; }
        public string MgrEmployeeId { get; set; }

        #endregion

        #region Extended Properties

        public string ExtendedName => Org + " - " + OrgDesc;
        #endregion

        #region Constructor

        public OrgListObject() { }

        #endregion

        #region Repository

        private static IOrgListRepository repo => new OrgListRepository();

        #endregion

        #region Static Methods

        public static List<OrgListObject> GetOrgs() => repo.GetOrgs();

        public static List<OrgListObject> GetUserOrgs(string employeeId) => repo.GetUserOrgs(employeeId);

        public static OrgListObject Getorg(int employerCode) => repo.GetOrg(employerCode);

        public static OrgListObject GetOrg(string org) => repo.GetOrg(org);

        #endregion

        #region Object Methods

        #endregion
    }

    public interface IOrgListRepository
    {
        List<OrgListObject> GetOrgs();
        List<OrgListObject> GetUserOrgs(string employeeId);
        OrgListObject GetOrg(int employerCode);
        OrgListObject GetOrg(string org);
    }

    public class OrgListRepository : IOrgListRepository
    {
        public List<OrgListObject> GetOrgs() => Config.EmployeeConn.Query<OrgListObject>("SELECT * FROM Orgs").ToList();

        public List<OrgListObject> GetUserOrgs(string employeeId)
        {
            var org = EmployeeCache.GetHomeOrg(employeeId) == null ? EmployeeCache.GetWorkOrg(employeeId).Substring(0, 2).ToUpper() : EmployeeCache.GetHomeOrg(employeeId).Substring(0, 2).ToUpper();

            return Config.EmployeeConn.Query<OrgListObject>("SELECT * FROM Orgs WHERE Org = @Org", new { Org = org }).ToList();
        }

        public OrgListObject GetOrg(int employerCode) => Config.EmployeeConn.Query<OrgListObject>("SELECT * FROM Orgs WHERE EmployerCode = @EmployerCode", new { EmployerCode = employerCode }).FirstOrDefault();

        public OrgListObject GetOrg(string org) => Config.EmployeeConn.Query<OrgListObject>("SELECT * FROM Orgs WHERE Org = @Org", new { Org = org }).FirstOrDefault();
    }
}
