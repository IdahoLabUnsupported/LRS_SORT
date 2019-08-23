using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LRS.Business;

namespace LRS.EmployeeHelper
{
    public class EmployeeSearchObject
    {
        #region Properties

        public string EmployeeId { get; set; }
        public string NameFull { get; set; }

        #endregion

        #region Static Methods

        internal static string ConnectionString
        {
            get
            {
                string key = "EmployeeConnection";
                var appKey =  ConfigurationManager.AppSettings["EmployeeConnection"];
                if (appKey != null)
                    key = appKey.ToString();
                return ConfigurationManager.ConnectionStrings[key].ToString();
            }
        }

        public static IEnumerable<EmployeeSearchObject> Search(string text) => Search(text, false);

        public static IEnumerable<EmployeeSearchObject> Search(string text, bool all) => Config.EmployeeManager.SearchEmployee(text, all)?.Select(n => new EmployeeSearchObject(){EmployeeId = n.EmployeeId, NameFull = n.FullName});

        public static EmployeeSearchObject Get(string employeeId)
        {
            var sp = EmployeeCache.GetEmployee(employeeId);
            return new EmployeeSearchObject
            {
                EmployeeId = sp.EmployeeId,
                NameFull = sp.FullName
            };
        }

        public static IEnumerable<EmployeeSearchObject> Get(List<string> employeIds)
        {
            List<EmployeeSearchObject> toReturn = new List<EmployeeSearchObject>();
            foreach (var employeId in employeIds)
                toReturn.Add(Get(employeId));
            return toReturn;
        }

        #endregion
    }
}
