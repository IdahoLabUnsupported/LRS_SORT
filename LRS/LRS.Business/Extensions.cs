using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Inl.MvcHelper;

namespace LRS.Business
{
    public static class Extensions
    {
        public static string FirstName(this string item)
        {
            string[] names = item.Split(' ');
            if (names.Length > 0)
            {
                return names[0];
            }

            return item;
        }

        public static string MiddelName(this string item)
        {
            string[] names = item.Split(' ');
            if (names.Length > 2)
            {
                return names[1];
            }

            return string.Empty;
        }

        public static string LastName(this string item)
        {
            string[] names = item.Split(' ');
            if (names.Length > 3)
            {
                string name = string.Empty;
                for (int i = 2; i < names.Length; i++)
                {
                    name += (' ' + names[i]);
                }

                return name.Trim();
            }
            if (names.Length > 1)
            {
                return names[names.Length - 1];
            }

            return string.Empty;
        }

        public static string TrailingSlash(this string item)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                if (item[item.Length - 1] != '/')
                {
                    item += "/";
                }
            }

            return item;
        }

        public static SelectList GetEnumSelectList<T>(string[] exclusions = null) where T : struct, IConvertible
        {
            if (typeof(T).IsEnum)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();

                List<string> ex = new List<string>();
                if (exclusions != null)
                {
                    foreach (string x in exclusions)
                    {
                        ex.Add(x);
                    }
                }

                foreach (Enum pt in Enum.GetValues(typeof(T)))
                {
                    if (!ex.Exists(n => n.Equals(pt.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        data.Add(pt.ToString(), pt.GetEnumDisplayName());
                    }
                }

                return new SelectList(data.OrderBy(m => m.Value), "key", "value");
            }

            return null;
        }
    }
}
