﻿using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource
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
                default:
                    result = "Unknown";
                    break;
            }

            return result;
        }
    }
}
