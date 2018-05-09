using TT.StockQuoteSource.Contracts;

namespace TT.StockQuoteSource
{
    internal static class CountryExtensions
    {
        public static string GetShortName(this Country country)
        {
            string result;

            switch (country)
            {
                case Country.Taiwan:
                    result = "TW";
                    break;
                case Country.USA:
                    result = "US";
                    break;
                case Country.HK:
                    result = "HK";
                    break;
                default:
                    result = "Unknown";
                    break;
            }

            return result;
        }
    }
}
