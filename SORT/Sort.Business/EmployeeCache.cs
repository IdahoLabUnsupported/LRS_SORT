using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Sort.Interfaces;

namespace Sort.Business
{
    public class EmployeeCache
    {
        public static IEmployee GetEmployee(string employeeId, bool all = false)
        {
            if (string.IsNullOrWhiteSpace(employeeId)) return null;
            employeeId = employeeId.PadLeft(6, '0');
            string cacheKey = "Employees";
            var cache = HttpContext.Current.Cache;
            var data = (List<IEmployee>)cache[cacheKey];
            if (data == null)
            {
                data = Config.EmployeeManager.GetAllEmployees().ToList();
                if (data == null)
                    return null;
                cache.Insert(cacheKey, data, null, DateTime.UtcNow.AddMinutes(60), Cache.NoSlidingExpiration);
            }

            return data.FirstOrDefault(n => n.EmployeeId == employeeId);
        }

        public static string GetName(string employeeId) => GetEmployee(employeeId)?.PreferredName;

        public static string GetFullName(string employeeId) => GetEmployee(employeeId)?.FullName;

        public static string GetPreferredName(string employeeId) => GetEmployee(employeeId)?.PreferredName;

        public static string GetEmail(string employeeId) => GetEmployee(employeeId)?.Email;

        public static string GetWorkOrg(string employeeId) => GetEmployee(employeeId)?.WorkOrginization;

        public static string GetHomeOrg(string employeeId) => GetEmployee(employeeId)?.HomeOrginization;
    }
}
