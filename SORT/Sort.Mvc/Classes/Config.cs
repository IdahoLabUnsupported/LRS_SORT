using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Sort.Business;
using System.Web.Hosting;

namespace Sort.Mvc
{
    public class Config : Sort.Business.Config
    {

		#region SetDbConfigVals

		public static void SetDbConfigVals()
        {
            ConfigSetting applicationMode = new ConfigSetting("ApplicationMode", ApplicationMode.ToString());
			Sort.Business.Config.setValue(applicationMode);
        }


        public static UserObject User => UserObject.CurrentUser;
		#endregion

		#region Samples

		private static ConfigSetting sampleString = new ConfigSetting("SampleString", "This is a string");
		public static string SampleString
		{
			get { return getValue(sampleString); }
			set { setValue(sampleString.SetValue(value)); }
		}

		public static ConfigSetting sampleInt = new ConfigSetting("SampleInt", 9.ToString());
		public static int SampleInt
		{
			get { return int.Parse(getValue(sampleInt)); }
			set { setValue(sampleInt.SetValue(value.ToString())); }
		}

		public static ConfigSetting sampleBool = new ConfigSetting("SampleBool", true.ToString());
		public static bool SampleBool
		{
			get { return bool.Parse(getValue(sampleBool)); }
			set { setValue(sampleBool.SetValue(value.ToString())); }
		}

		#endregion
	}
}