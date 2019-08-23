using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Sort.Business
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the display name from the enum extended attribute.
        /// NOTE: If the enum does not have an Display Name attribute, this will return null.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetEnumDisplayName(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            if (member != null && member.Length > 0)
            {
                DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

                if (displayName != null)
                {
                    return displayName.Name;
                }
            }

            return item.ToString();
        }

        /// <summary>
        /// Gets the short name from the enum extended attribute.
        /// NOTE: If the enum does not have an Display Name attribute, this will return null.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetEnumShortName(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            if (member != null && member.Length > 0)
            {
                DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

                if (displayName != null)
                {
                    return displayName.ShortName;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the display description from the enum extended attribute.
        /// NOTE: If the enum does not have an Display Description attribute, this will return null.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            if (member != null && member.Length > 0)
            {
                DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

                if (displayName != null)
                {
                    return displayName.Description;
                }
            }

            return string.Empty;
        }

        public static string GetPropertyDisplayName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo != null)
            {
                var attr = memberInfo.GetAttribute<DisplayAttribute>();
                if (attr != null)
                {
                    return attr.Name;
                }
            }

            return null;
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression propertyExpression = (MemberExpression)expression.Body;
            MemberInfo propertyMember = propertyExpression.Member;

            Object[] displayAttributes = propertyMember.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (displayAttributes != null && displayAttributes.Length == 1)
                return ((DisplayAttribute)displayAttributes[0]).GetShortName();

            return propertyMember.Name;
        }

        public static string GetPropertyShortName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo != null)
            {
                var attr = memberInfo.GetAttribute<DisplayAttribute>();
                if (attr != null)
                {
                    return attr.ShortName;
                }
            }

            return null;
        }

        public static string GetPropertyDescription<T>(Expression<Func<T>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo != null)
            {
                var attr = memberInfo.GetAttribute<DisplayAttribute>();
                if (attr == null)
                {
                    return memberInfo.Name;
                }

                return attr.Description;
            }

            return null;
        }

        //public static SelectList GetEnumSelectList<T>() where T : struct, IConvertible
        //{
        //    if (typeof(T).IsEnum)
        //    {
        //        Dictionary<string, string> data = new Dictionary<string, string>();

        //        foreach (Enum pt in Enum.GetValues(typeof(T)))
        //        {
        //            data.Add(pt.ToString(), pt.GetEnumDisplayName());
        //        }

        //        return new SelectList(data.OrderBy(m => m.Value), "key", "value");
        //    }

        //    return null;
        //}

        public static T ToEnum<T>(this string item)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                try
                {
                    var a = Enum.Parse(typeof(T), item);

                    return (T) a;
                }
                catch { }
            }

            return default(T);
        }

        public static int ToInt(this string item)
        {
            int num;
            if (int.TryParse(item, out num))
            {
                return num;
            }

            return 0;
        }

        public static int? Revision(this string item)
        {
            int num;
            if (int.TryParse(item[item.Length-1].ToString(), out num))
            {
                return num;
            }

            return null;
        }

        public static string StiNumber(this string item)
        {
            int idx = item.IndexOf("Rev", StringComparison.InvariantCultureIgnoreCase);
            if (idx >= 0)
            {
                return item.Substring(0, idx - 1);
            }

            return null;
        }


        public static bool ToBool(this string item)
        {
            bool b;
            if (bool.TryParse(item, out b))
            {
                return b;
            }

            return false;
        }

        public static DateTime? ToDateTime(this string item)
        {
            DateTime d;
            if (DateTime.TryParse(item, out d))
            {
                return d;
            }

            return null;
        }

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
                return names[names.Length-1];
            }

            return string.Empty;
        }

        public static string TrailingForwardSlash(this string item)
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

        public static string TrailingBackSlash(this string item)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                if (item[item.Length - 1] != '\\')
                {
                    item += "\\";
                }
            }

            return item;
        }

        public static bool StringsAreEqual(string a, string b)
        {
            if (a == null)
            {
                if (b == null)
                {
                    return true;
                }

                return false;
            }

            if (b == null)
            {
                return false;
            }

            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        private static T GetAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();
            
            return (T)attribute;
        }

        private static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }

        public static string RemoveNonAscii(this string value)
        {
            string pattern = "[^ -~]+";
            Regex reg_exp = new Regex(pattern);
            return reg_exp.Replace(value, "");
        }
    }

    public static class WebRequestExtensions
    {
        public static WebResponse GetResponseWithoutException(this WebRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            try
            {
                return request.GetResponse();
            }
            catch (WebException ex)
            {
                ErrorLogObject.LogError("InlDigitalLibrary::GetResponseWithoutException", ex);
                return ex.Response;
            }
        }
    }
}
