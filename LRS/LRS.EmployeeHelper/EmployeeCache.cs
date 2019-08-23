using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using LRS.Interfaces;
using Config = LRS.Business.Config;

namespace LRS.EmployeeHelper
{
    public class EmployeeCache
    {
        public static IEmployee GetEmployee(string employeeId, bool all = false)
        {
            if (string.IsNullOrWhiteSpace(employeeId)) return null;
            employeeId = employeeId.PadLeft(6, '0');
            string cacheKey = $"EmployeeCache_{employeeId}";
            var cache = HttpContext.Current.Cache;
            var data = (List<IEmployee>)cache[cacheKey];
            if (data == null)
            {
                data = Config.EmployeeManager.GetAllEmployees().ToList();
                if (data == null)
                    return null;
                cache.Insert(cacheKey, data, null, DateTime.UtcNow.AddHours(8), Cache.NoSlidingExpiration);
            }
            if (data == null)
                throw new Exception("Invalid Employee ID provided or no user with that Employee ID was found");

            return data.FirstOrDefault(n => n.EmployeeId == employeeId);
        }

        public static string GetName(string employeeId) => GetEmployee(employeeId)?.PreferredName;

        public static string GetEmail(string employeeId) => GetEmployee(employeeId)?.Email;
    }
}
