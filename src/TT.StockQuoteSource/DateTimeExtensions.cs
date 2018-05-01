using System;
using System.Globalization;
using TT.StockQuoteSource.Contracts;

namespace TT.StockQuoteSource
{
    internal static class DateTimeExtensions
    {
        private static readonly string _taiwanZoneId = "Taipei Standard Time";
        private static readonly string _usaEastZoneId = "US Eastern Standard Time";

        public static string ToUnixTimestamp(this DateTime date)
        {
            DateTime unixStartDate = new DateTime(1970, 1, 1, 0, 0, 0);
            return (date - unixStartDate).TotalSeconds.ToString(CultureInfo.InvariantCulture);
        }

        public static bool IsSameDay(this DateTime date, DateTime compare)
        {
            if (date.Year == compare.Year && date.Month == compare.Month && date.Day == compare.Day)
            {
                return true;
            }
            return false;
        }

        public static DateTime ToCountryLocalTime(this DateTime UTCTime, Country country)
        {
            TimeZoneInfo timeZoneInfo;

            switch (country)
            {
                case Country.Taiwan:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_taiwanZoneId);
                    break;
                case Country.USA:
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_usaEastZoneId);
                    break;
                default:
                    return UTCTime;
            }

            return TimeZoneInfo.ConvertTimeFromUtc(UTCTime, timeZoneInfo);
        }
    }
}
