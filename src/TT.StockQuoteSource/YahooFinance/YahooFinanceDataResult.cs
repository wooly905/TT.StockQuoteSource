using System;
using TT.StockQuoteSource.Contracts;

namespace TT.StockQuoteSource.YahooFinance
{
    /// <summary>
    /// Class of YahooFinanceDataResult
    /// </summary>
    public class YahooFinanceDataResult : StockQuoteResult
    {
        /// <summary>
        /// ctor of YahooFinanceDataResult
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="date"></param>
        /// <param name="lowPrice"></param>
        /// <param name="highPrice"></param>
        /// <param name="closePrice"></param>
        /// <param name="openPrice"></param>
        /// <param name="volume"></param>
        public YahooFinanceDataResult(Country country, string stockId, DateTime date, decimal lowPrice, decimal highPrice, decimal closePrice, decimal openPrice, int volume)
            : base(country, stockId, date, lowPrice, highPrice, closePrice, openPrice, volume)
        {
        }

        /// <summary>
        /// sesssion string for getting historical data from Yahoo Finance
        /// </summary>
        public string Crumb { get; set; }
    }
}
