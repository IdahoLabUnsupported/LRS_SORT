using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LRS.Business
{
    public class UserOrgObject
    {
        #region Properties

        public int UserOrgId { get; set; }
        public string EmployeeId { get; set; }
        public string Org { get; set; }
        #endregion

        #region Extended Properties

        public string OrgDesc => Config.EmployeeManager.GetOrganization(Org)?.Description;
        public string OrgDisplayName => $"{Org} - {OrgDesc}";

        #endregion

        #region Constructor

        public UserOrgObject() { }

        #endregion

        #region Repository

        private static IUserOrgRepository repo => new UserOrgRepository();

        #endregion

        #region Static Methods

        public static List<UserOrgObject> GetUserOrgs(string EmployeeId) => repo.GetUserOrgs(EmployeeId);

        public static List<string> GetUserOrgNames(string EmployeeId) => repo.GetUserOrgNames(EmployeeId);

        public static UserOrgObject GetUserOrg(int userOrgId) => repo.GetUserOrg(userOrgId);

        public static List<Organization> GetAvailableOrgs() => Config.EmployeeManager.GetAllOrganizations().Select(n => new Organization() {Org = n.OrgId, OrgDesc = n.Description}).ToList();

        public static SelectList AvailableOrgSelectList() => new SelectList(GetAvailableOrgs(), "Org", "OrgDisplayName");
        #endregion

        #region Object Methods

        public void Save()
        {
            repo.SaveUserOrg(this);
        }

        public bool Delete()
        {
            return repo.DeleteUserOrg(this);
        }

        #endregion
    }

    public interface IUserOrgRepository
    {
        List<UserOrgObject> GetUserOrgs(string employeeId);
        List<string> GetUserOrgNames(string employeeId);
        UserOrgObject GetUserOrg(int userOrgId);
        UserOrgObject SaveUserOrg(UserOrgObject userOrg);
        bool DeleteUserOrg(UserOrgObject userOrg);
    }

    public class UserOrgRepository : IUserOrgRepository
    {
        public List<UserOrgObject> GetUserOrgs(string employeeId) => Config.Conn.Query<UserOrgObject>("SELECT * FROM dat_UserOrg WHERE EmployeeId = @EmployeeId ORDER BY Org", new { EmployeeId = employeeId }).ToList();

        public List<string> GetUserOrgNames(string employeeId) => Config.Conn.Query<string>("SELECT Org FROM dat_UserOrg WHERE EmployeeId = @EmployeeId", new { EmployeeId = employeeId }).ToList();

        public UserOrgObject GetUserOrg(int userOrgId) => Config.Conn.Query<UserOrgObject>("SELECT * FROM dat_UserOrg WHERE UserOrgId = @UserOrgId", new { UserOrgId = userOrgId }).FirstOrDefault();

        public UserOrgObject GetUserOrg(string employeeId, string org) => Config.Conn.Query<UserOrgObject>("SELECT * FROM dat_UserOrg WHERE EmployeeId = @EmployeeId AND Org = @Org", new { EmployeeId = employeeId, Org  = org}).FirstOrDefault();

        public UserOrgObject SaveUserOrg(UserOrgObject userOrg)
        {
            if (userOrg.UserOrgId > 0) // Update
            {
                string sql = @"
                    UPDATE  dat_UserOrg
                    SET     EmployeeId = @EmployeeId,
                            Org = @Org
                    WHERE   UserOrgId = @UserOrgId";
                Config.Conn.Execute(sql, userOrg);
            }
            else
            {
                if (GetUserOrg(userOrg.EmployeeId, userOrg.Org) == null)
                {
                    string sql = @"
                    INSERT INTO dat_UserOrg (
                        EmployeeId,
                        Org
                    )
                    VALUES (
                        @EmployeeId,
                        @Org
                    )
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                    userOrg.UserOrgId = Config.Conn.Query<int>(sql, userOrg).Single();
                }
            }
            return userOrg;
        }

        public bool DeleteUserOrg(UserOrgObject userOrg)
        {
            try
            {
                Config.Conn.Execute("DELETE FROM dat_UserOrg WHERE UserOrgId = @UserOrgId", userOrg);
            }
            catch { return false; }
            return true;
        }
    }

    public class Organization
    {
        public string Org { get; set; }
        public string OrgDesc { get; set; }

        public string OrgDisplayName => $"{Org} - {OrgDesc}";
    }
}
