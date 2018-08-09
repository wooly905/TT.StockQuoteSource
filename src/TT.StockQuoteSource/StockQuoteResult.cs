using System;
using TT.StockQuoteSource.Contracts;

namespace TT.StockQuoteSource
{
    /// <summary>
    /// Data structure of stock quote
    /// </summary>
    public class StockQuoteResult : IStockQuoteFromDataSource
    {
        /// <summary>
        /// Http response content string
        /// </summary>
        protected string ResponseContentFromDataSource;

        /// <summary>
        /// Action to write log for exception
        /// </summary>
        protected Action<Exception> WriteToErrorLogAction;

        /// <summary>
        /// ctor of StockQuoteResult
        /// </summary>
        public StockQuoteResult()
        {
        }

        /// <summary>
        /// ctor of StockQuoteResult
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="date"></param>
        /// <param name="lowPrice"></param>
        /// <param name="highPrice"></param>
        /// <param name="closePrice"></param>
        /// <param name="openPrice"></param>
        /// <param name="volume"></param>
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

        /// <summary>
        /// Low price
        /// </summary>
        public decimal LowPrice { get; set; }

        /// <summary>
        /// High price
        /// </summary>
        public decimal HighPrice { get; set; }

        /// <summary>
        /// Close price, also current price
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// Open price
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// Trade date time
        /// </summary>
        public DateTime TradeDateTime { get; set; }

        /// <summary>
        /// Trading volume
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Stock symbol
        /// </summary>
        public string StockId { get; set; }

        /// <summary>
        /// Stock name
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// Country of a stock
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        /// Get validality of a stock quote data
        /// </summary>
        public bool IsValid => LowPrice > 0
                               && HighPrice > 0
                               && ClosePrice > 0
                               && OpenPrice > 0
                               && Volume >= 0
                               && !string.IsNullOrEmpty(StockId);
    }
}
