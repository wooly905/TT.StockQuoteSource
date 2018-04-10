using System;
using System.Globalization;

namespace TTStockQuoteSource
{
    internal static class DateTimeExtensions
    {
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
    }
}
