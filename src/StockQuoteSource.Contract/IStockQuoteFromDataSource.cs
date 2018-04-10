using System;

namespace TTStockQuoteSource.Contracts
{
    public interface IStockQuoteFromDataSource
    {
        Country Country { get; }
        string StockId { get; }
        decimal LowPrice { get; }
        decimal HighPrice { get; }
        decimal ClosePrice { get; }  // a.k.a. Latest Price
        decimal OpenPrice { get; }
        DateTime TradeDateTime { get; }
        int Volume { get; }
        bool IsValid { get; }
    }
}
