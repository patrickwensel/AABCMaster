using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Dymeng.Framework.Strings
{
    public static class Extensions
    {

        /// <summary>
        /// Replaces string content at the specified index and length
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index">index at which to start replacing characters</param>
        /// <param name="replaceWith">string to be inserted</param>
        /// <param name="charsToRemove">number of characters to remove from the existing string</param>
        /// <returns></returns>
        public static string ReplaceAtIndex(this string str, int index, string replaceWith, int charsToRemove = 0) {

            var builder = new StringBuilder(str);
            builder.Remove(index, charsToRemove);
            builder.Insert(index, replaceWith);
            str = builder.ToString();
            return str;

        }




        public static DateTime ISOStringToDateTime(this string isoString) {
            return DateTime.Parse(isoString, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        public static DateTime? ISOStringToDateTimeOrNull(this string isoString) {
            DateTime d;
            if (!DateTime.TryParse(isoString, null, System.Globalization.DateTimeStyles.RoundtripKind, out d)) {
                return null;
            } else {
                return d;
            }
        }

        public static TimeSpan? ISOStringToTimeSpanOrNull(this string isoString) {

            DateTime d;
            if (!DateTime.TryParse(isoString, null, System.Globalization.DateTimeStyles.RoundtripKind, out d)) {
                return null;
            }

            return d.TimeOfDay;

        }


        public static int NthIndexOf(this string target, string value, int n) {
            Match m = Regex.Match(target, "((" + Regex.Escape(value) + ").*?){" + n + "}");

            if (m.Success)
                return m.Groups[2].Captures[n - 1].Index;
            else
                return -1;
        }
    }
}
