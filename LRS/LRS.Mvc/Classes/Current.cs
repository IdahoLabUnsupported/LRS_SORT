using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LRS.Business;

namespace LRS.Mvc
{
    public class Current
    {
        public static UserObject User => UserObject.CurrentUser;
        public static bool IsAdmin => User.IsInAnyRole("Admin");
        public static bool IsReleaseOfficer => User.IsInAnyRole("Admin,ReleaseOfficial");
        public static bool IsDoeUser => User.IsInAnyRole("Admin,ReleaseOfficial,DoeUser");
        public static bool IsHistorical => User.IsInAnyRole("Admin,ReleaseOfficial,HistWorker");
    }
}