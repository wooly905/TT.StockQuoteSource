using System;
using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource
{
    public class StockQuoteResult : IStockQuoteFromDataSource
    {
        protected string ResponseContentFromDataSource;
        protected Action<Exception> WriteToErrorLogAction;

        public StockQuoteResult()
        {
        }

        public StockQuoteResult(Country country, string stockId, DateTime date, decimal lowPrice, decimal highPrice, decimal closePrice, decimal openPrice, int volume)
        {
            Country = country;
            StockId = stockId;
            TradeDateTime = date;
            LowPrice = lowPrice;
            HighPrice = highPrice;
            ClosePrice = closePrice;
            OpenPrice = openPrice;
            Volume = volume;
        }

        public decimal LowPrice { get; set; }

        public decimal HighPrice { get; set; }

        public decimal ClosePrice { get; set; }

        public decimal OpenPrice { get; set; }

        public DateTime TradeDateTime { get; set; }

        public int Volume { get; set; }

        public string StockId { get; set; }

        public string StockName { get; set; }

        public Country Country { get; set; }

        public bool IsValid => LowPrice > 0 && HighPrice > 0 && ClosePrice > 0 && OpenPrice > 0 && Volume >= 0 && !string.IsNullOrEmpty(StockId);
    }
}
