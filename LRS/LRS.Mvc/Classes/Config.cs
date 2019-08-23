using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using LRS.Business;
using System.Web.Hosting;

namespace LRS.Mvc
{
    public class Config : LRS.Business.Config
    {

		#region SetDbConfigVals

		public static void SetDbConfigVals()
        {
            LRS.Business.Config.setValue(new ConfigSetting("ApplicationMode", ApplicationMode.ToString()));
        }

		#endregion	
	}
}