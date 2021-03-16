using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;

namespace BoincRpc
{
    internal static class Utils
    {
        public static string GetMD5Hash(string s)
        {
            byte[] hash;

            using (MD5 md5 = MD5.Create())
                hash = md5.ComputeHash(Encoding.ASCII.GetBytes(s));

            StringBuilder hashString = new StringBuilder(hash.Length * 2);

            for (int i = 0; i < hash.Length; i++)
                hashString.Append(hash[i].ToString("x2"));

            return hashString.ToString();
        }

        public static DateTimeOffset ConvertUnixTimeToDateTime(double unixTime)
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(unixTime);
        }

        public static TimeSpan ConvertSecondsToTimeSpan(double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        public static double ConvertTimeSpanToSeconds(TimeSpan timeSpan)
        {
            return timeSpan.TotalSeconds;
        }

        //adapted code from http://www.blackbeltcoder.com/Articles/time/creating-a-user-friendly-timespan-string 
        public static string ToUserFriendlyString(this TimeSpan span)
        {
            const int DaysInYear = 365;
            const int DaysInMonth = 30;

            // Get each non-zero value from TimeSpan component
            List<string> values = new List<string>();

            // Number of years
            int days = span.Days;
            double totaldays = span.TotalDays;
            if (days >= DaysInYear)
            {
                double years = (days / DaysInYear);
                return $"{Math.Round(years, 2).ToString()} years";
            }
            // Number of months
            if (days >= DaysInMonth)
            {
                double months = (days / DaysInMonth);
                return $"{Math.Round(months, 2).ToString()} months";
            }
            // Number of days
            if (days >= 1)
                return $"{Math.Round(totaldays, 2)} days";
            // Number of hours
            if (span.Hours >= 1)
                return $"{Math.Round(span.TotalHours, 2)} hours";
            // Number of minutes
            if (span.Minutes >= 1)
                return $"{Math.Round(span.TotalMinutes, 2)} minutes";
            if (span.Seconds >= 1 || values.Count == 0)
                return $"{span.Seconds} seconds";
            return "Unknown";
        }

        /// <summary>
        /// Constructs a string description of a time-span value.
        /// </summary>
        /// <param name="value">The value of this item</param>
        /// <param name="description">The name of this item (singular form)</param>
        private static string CreateValueString(int value, string description)
        {
            return String.Format("{0:#,##0} {1}",
                value, (value == 1) ? description : String.Format("{0}s", description));
        }
    }

    internal static class ExtensionsMethods
    {
        public static bool ContainsElement(this XElement element, XName name)
        {
            return element.Element(name) != null;
        }

        public static bool ElementBoolean(this XElement element, XName name, bool defaultValue = default(bool))
        {
            XElement child = element.Element(name);

            if (child == null)
                return defaultValue;
            else
                if (child.IsEmpty)
                    return true;
                else
                    return (bool)child;
        }

        public static int ElementInt(this XElement element, XName name, int defaultValue = default(int))
        {
            return ((int?)element.Element(name)).GetValueOrDefault(defaultValue);
        }

        public static double ElementDouble(this XElement element, XName name, double defaultValue = default(double))
        {
            return ((double?)element.Element(name)).GetValueOrDefault(defaultValue);
        }

        public static string ElementString(this XElement element, XName name, string defaultValue = default(string))
        {
            return (string)element.Element(name) ?? defaultValue;
        }

        public static DateTimeOffset ElementDateTimeOffset(this XElement element, XName name, DateTimeOffset defaultValue = default(DateTimeOffset))
        {
            double? t = (double?)element.Element(name);

            if (t == null)
                return defaultValue;
            else
                return Utils.ConvertUnixTimeToDateTime(t.Value);
        }

        public static TimeSpan ElementTimeSpan(this XElement element, XName name, TimeSpan defaultValue = default(TimeSpan))
        {
            double? t = (double?)element.Element(name);

            if (t == null)
                return defaultValue;
            else
                return Utils.ConvertSecondsToTimeSpan(t.Value);
        }
    }
}
