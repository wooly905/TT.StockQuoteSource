using System;

namespace TT.StockQuoteSource.Contracts
{
    /// <summary>
    /// IStockQuoteFromDataSource
    /// </summary>
    public interface IStockQuoteFromDataSource
    {
        /// <summary>
        /// Country
        /// </summary>
        Country Country { get; }

        /// <summary>
        /// Stock symbol
        /// </summary>
        string StockId { get; }

        /// <summary>
        /// Low price
        /// </summary>
        decimal LowPrice { get; }

        /// <summary>
        /// High price
        /// </summary>
        decimal HighPrice { get; }

        /// <summary>
        /// Close price, also current price
        /// </summary>
        decimal ClosePrice { get; }

        /// <summary>
        /// Open price
        /// </summary>
        decimal OpenPrice { get; }

        /// <summary>
        /// Trade date time
        /// </summary>
        DateTime TradeDateTime { get; }

        /// <summary>
        /// Trading volume
        /// </summary>
        int Volume { get; }

        /// <summary>
        /// Get validality of a stock quote data
        /// </summary>
        bool IsValid { get; }
    }
}
