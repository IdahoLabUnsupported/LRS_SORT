using LRS.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LRS.Mvc.Classes
{
    public class Developer
    {
        private static Dictionary<string, string> DeveloperRoles = new Dictionary<string, string>();

        public static string GetDeveloperRole()
        {
            if (Config.ExecutionMode == LRS.Business.ExecutionMode.Local)
            {
                string toReturn = null;
                DeveloperRoles.TryGetValue(UserObject.CurrentUser.EmployeeId, out toReturn);
                return toReturn;
            }
            else
            {
                return null;
            }
        }

        public static void SetDeveloperRole(string role)
        {
            if (Config.ExecutionMode == LRS.Business.ExecutionMode.Local)
            {
                if (!string.IsNullOrEmpty(role))
                    DeveloperRoles[UserObject.CurrentUser.EmployeeId] = role;
                else
                {
                    if (DeveloperRoles.Keys.Contains(UserObject.CurrentUser.EmployeeId))
                    {
                        DeveloperRoles.Remove(UserObject.CurrentUser.EmployeeId);
                    }
                }
            }
        }
    }
}