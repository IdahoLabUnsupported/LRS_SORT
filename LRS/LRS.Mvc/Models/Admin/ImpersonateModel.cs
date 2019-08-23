using Inl.MvcHelper.Extensions;
using LRS.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LRS.Mvc.Models.Admin
{
    public class ImpersonateModel
    {
        #region Properties

        private string employeeId;
        [Display(Name = "Employee")]
        public string EmployeeId
        {
            get
            {
                return employeeId;
            }
            set
            {
                employeeId = value;
                if (Config.ExecutionMode == ExecutionMode.Local)
                {
                    List<string> employeeIds = new List<string>();
                    HttpCookie cookie = HttpContext.Current.Request.Cookies["RecentImpersonations"];
                    if (cookie != null)
                    {
                        string recents = cookie.Value;
                        employeeIds = recents.SplitClean();
                        employeeIds.Remove(value);
                    }
                    employeeIds.Insert(0, value);
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("RecentImpersonations", string.Join(",", employeeIds)));
                }
            }
        }

        #endregion

        #region Lists

        public List<KeyValuePair<string, string>> PreviousImpersonations
        {
            get
            {
                if (Config.ExecutionMode == LRS.Business.ExecutionMode.Local)
                {
                    List<KeyValuePair<string, string>> toReturn = new List<KeyValuePair<string, string>>();
                    HttpCookie cookie = HttpContext.Current.Request.Cookies["RecentImpersonations"];
                    if (cookie != null)
                    {
                        string recents = cookie.Value;
                        List<string> employeeIds = recents.SplitClean();

                        foreach (string employeeId in employeeIds)
                        {
                            toReturn.Add(new KeyValuePair<string, string>(employeeId, EmployeeCache.GetFullName(employeeId)));
                        }
                    }

                    return toReturn;
                }
                return null;
            }
        }

        #endregion
    }
}