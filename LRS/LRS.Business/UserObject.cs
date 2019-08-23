using Dapper;
using Inl.MvcHelper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Inl.MvcHelper;
using LRS.Interfaces;

namespace LRS.Business
{
    public class UserObject
    {
        #region Properties

        public string EmployeeId { get; set; }

        private List<string> _roles = null;
        public List<string> Roles {
            get
            {
                if (_roles == null)
                {
                    _roles = GetUserRoles(EmployeeId);
                }

                return _roles;
            }
        } 
        
        public bool Impersonating { get; private set; }

        #endregion

        #region Extended Properties

        private IEmployee _employee;
        protected IEmployee Employee
        {
            get => _employee ?? Config.EmployeeManager.GetEmployeeById(EmployeeId, false);
            set => _employee = value;
        }
        public string UserName { get; set; }
        public List<string> ManagerOrgs => (Roles.Contains(UserRole.OrgManager.ToString()) ? UserOrgObject.GetUserOrgNames(EmployeeId) : new List<string>()); 
        #endregion

        #region Derived Properties
        
        public string FullName => Employee.FullName;
        public string Email => Employee.Email;

        #endregion

        #region Constructor

        public UserObject() { }

        public UserObject(IEmployee employee)
        {
            if (employee != null)
            {
                EmployeeId = employee.EmployeeId;
                UserName = employee.UserNetworkId;
            }
        }
        #endregion

        #region Static Methods

        private static UserObject getByUserName(string username)
        {
            IEmployee employee = Config.EmployeeManager.GetEmployeeByUserName(username, false);
            if (employee == null)
                return null;
            else
            {
                UserObject user = new UserObject
                {
                    EmployeeId = employee.EmployeeId,
                    UserName = username,
                    _employee = employee
                };
                return user;
            }
        }

        internal static List<string> GetUserRoles(string EmployeeId) => Config.Conn.Query<string>("SELECT DISTINCT Role FROM dat_User WHERE EmployeeId = @EmployeeId", new {EmployeeId = EmployeeId}).ToList();

        private static UserObject getUserFromCache(string username)
        {
            var cacheKey = $"{username}:LRS";
            var cache = HttpContext.Current.Cache;
            var user = (UserObject)cache[cacheKey];
            if (user == null)
            {
                user = getByUserName(username);
                if (user == null)
                    throw new AccessViolationException("Unknown user " + username);
                cache.Insert(cacheKey, user, null, DateTime.UtcNow.AddMinutes(Config.ApplicationMode == ApplicationMode.Production ? 5 : 1), Cache.NoSlidingExpiration);
            }
            return user;
        }

        public static UserObject CurrentUser
        {
            get
            {
                try
                {
                    bool impersonating = false;
                    string currentUser = HttpContext.Current.User.Identity.Name;
                    if (HttpContext.Current.Session["ImpersonateAs"] != null)
                    {
                        currentUser = HttpContext.Current.Session["ImpersonateAs"].ToString();
                        impersonating = true;
                    }

                    var username = currentUser.Substring(currentUser.LastIndexOf(@"\", StringComparison.InvariantCultureIgnoreCase) + 1);
                    UserObject toReturn = getUserFromCache(username);
                    toReturn.Impersonating = impersonating;
                    return toReturn;
                }
                catch { }

                return new UserObject();
            }
        }

        public static UserObject RealCurrentUser
        {
            get
            {
                string currentUser = HttpContext.Current.User.Identity.Name;
                var username = currentUser.Substring(currentUser.LastIndexOf(@"\", StringComparison.InvariantCultureIgnoreCase) + 1);
                UserObject toReturn = getUserFromCache(username);
                toReturn.Impersonating = false;
                return toReturn;
            }
        }

        public static UserObject GetUser(string EmployeeId)
        {
            UserObject toReturn = Config.Conn.Query<UserObject>("SELECT EmployeeId, UserID AS UserName FROM vw_User WHERE EmployeeId = @EmployeeId", new { EmployeeId = EmployeeId }).FirstOrDefault();
            if (toReturn == null)
            {
                var data = Config.EmployeeManager.GetEmployeeById(EmployeeId, false);
                if (data != null)
                {
                    toReturn = new UserObject(data);
                }
            }

            return toReturn;
        }

        public static List<UserObject> GetUsers()
        {
            return Config.Conn.Query<UserObject>("SELECT EmployeeId, UserID AS UserName FROM vw_User").ToList();
        }

        #endregion

        #region Object Methods

        public void Save()
        {
            var roles = Roles;
            Config.Conn.Execute("DELETE FROM dat_User WHERE EmployeeId = @EmployeeId", this);
            roles.ForEach(n => Config.Conn.Execute("INSERT INTO dat_User(EmployeeId, Role) VALUES(@EmployeeId, @RoleString)", new {EmployeeId = EmployeeId, RoleString = n.ToString()}));
            ClearCache();
        }

        public void Delete()
        {
            Config.Conn.Execute(@"DELETE FROM dat_UserOrg WHERE EmployeeId = @EmployeeId", this);
            Config.Conn.Execute(@"DELETE FROM dat_User WHERE EmployeeId = @EmployeeId", this);
            ClearCache();
        }

        private void ClearCache()
        {
            var cacheKey = $"{UserName}:LRS";
            HttpContext.Current.Cache.Remove(cacheKey);
        }

        public bool IsInAnyRole(string roles) => Roles.Any(u => roles.SplitClean().Contains(u));

        #endregion
    }

    public class UserListObject
    {
        #region Properties

        public string EmployeeId { get; set; }
        internal List<string> UserRoles { get; set; } = new List<string>();
        public string RoleString { get; set; }
        public string RoleDisplayString => string.Join(", ", UserRoles.OrderBy(m => m).Select(n => n.ToEnum<UserRole>().GetEnumDisplayName()));
        public string NameFull { get; set; }

        #endregion

        #region Static Methods

        public static List<UserListObject> GetUsers()
        {
            var users = Config.Conn.Query<UserListObject>("SELECT DISTINCT EmployeeId, NameFull FROM vw_User ORDER BY EmployeeId").ToList();
            foreach (var u in users)
            {
                u.UserRoles.AddRange(UserObject.GetUserRoles(u.EmployeeId));
            }

            return users;
        }

        #endregion
    }
}
