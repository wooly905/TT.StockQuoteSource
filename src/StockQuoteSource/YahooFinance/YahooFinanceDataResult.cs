using System;
using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource.YahooFinance
{
    public class YahooFinanceDataResult : StockQuoteResult
    {
        public YahooFinanceDataResult(Country country, string stockId, DateTime date, decimal lowPrice, decimal highPrice, decimal closePrice, decimal openPrice, int volume)
            : base(country, stockId, date, lowPrice, highPrice, closePrice, openPrice, volume)
        {
        }

        public string Crumb { get; set; }
    }
}
