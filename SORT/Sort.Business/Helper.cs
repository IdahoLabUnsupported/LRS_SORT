using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Sort.Business
{
    public static class Helper
    {
        public static int UnitedStatesCountryId => 250;

        public static SelectList FiscalYears
        {
            get
            {
                List<int> years = new List<int>();
                for (int i = DateTime.Now.AddMonths(3).Year; i >= DateTime.Now.AddMonths(3).AddYears(-6).Year; i--)
                {
                    years.Add(i);
                }

                return new SelectList(years);
            }
        }

        public static SelectList OrgList => new SelectList(Config.EmployeeManager.GetAllOrganizations().OrderBy(x => x.OrgId).ToList(), "OrgId", "DisplayName");

    }
}
