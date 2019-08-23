using LRS.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;

namespace LRS.Mvc.Security
{
	public class WindowsTokenCacheRoleProvider : WindowsTokenRoleProvider
	{
        public override string[] GetRolesForUser(string username)
        {
            UserObject currentUser = UserObject.CurrentUser;
            List<string> toReturn = new List<string>();
            if (Config.ExecutionMode == ExecutionMode.Local)
            {
                string developerRole = Classes.Developer.GetDeveloperRole();
                if (!string.IsNullOrEmpty(developerRole))
                    toReturn.Add(developerRole);
                else if (currentUser.Roles.Count > 0)
                    toReturn.AddRange(currentUser.Roles);
            }
            else if (currentUser.Roles.Count > 0)
            {
                toReturn.AddRange(currentUser.Roles);
            }
            
            if (currentUser.Impersonating)
                toReturn.Add("Impersonating");

            return toReturn.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
		{
			var roles = GetRolesForUser(username).ToList();
			return roles.Any(r => r.Trim().ToUpper().Contains(roleName.Trim().ToUpper()));
		}
	}
}