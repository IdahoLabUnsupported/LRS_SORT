using System;
using System.Collections.Generic;
using System.Linq;


namespace Sort.EmployeeHelper
{
    internal static class StringHelpers
    {
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrWhiteSpace(s);

        public static List<string> SplitClean(this string s) => Array.ConvertAll(s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), p => p.Trim()).ToList();

        public static string SplitCamelCase(this string input)
        {
            string toReturn = "";
            foreach (string part in System.Text.RegularExpressions.Regex.Split(input, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})"))
            {
                toReturn += part + " ";
            }
            return toReturn.Trim();
        }
    }
}
