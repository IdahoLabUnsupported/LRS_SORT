using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sort.Mvc.Classes
{
    public class Breadcrumb
    {
        #region Properties

        public static List<KeyValuePair<string, string>> Items
        {
            get
            {
                if (HttpContext.Current.Items["breadcrumb"] == null)
                {
                    HttpContext.Current.Items["breadcrumb"] = new List<KeyValuePair<string, string>>();
                }
                return (List<KeyValuePair<string, string>>)HttpContext.Current.Items["breadcrumb"];
            }
            set
            {
                HttpContext.Current.Items["breadcrumb"] = value;
            }
        }

        #endregion

        #region Static Methods

        public static Breadcrumb Auto()
        {
            Items = Menu.AutoBreadcrumb;
            return new Breadcrumb();
        }

        public static Breadcrumb Short()
        {
            Items = Menu.AutoShortBreadcrumb;
            return new Breadcrumb();
        }

        public static Breadcrumb Add(string text, string href)
        {
            Items.Add(new KeyValuePair<string, string>(text, href));
            return new Breadcrumb();
        }

        public static Breadcrumb Add(string text)
        {
            return Add(text, "");
        }

        #endregion

        #region Object Methods

        public Breadcrumb Next(string text, string href)
        {
            Items.Add(new KeyValuePair<string, string>(text, href));
            return this;
        }

        public Breadcrumb Next(string text)
        {
            return Next(text, "");
        }

        #endregion
    }
}