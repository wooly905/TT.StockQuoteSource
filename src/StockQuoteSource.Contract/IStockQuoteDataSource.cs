using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTStockQuoteSource.Contracts
{
    public interface IStockQuoteDataSource
    {
        StockQuoteSource Source { get; }
        Task<IStockQuoteFromDataSource> GetMostRecentQuoteAsync(Country country, string stockId, Action<Exception> writeToErrorLogAction);
        Task<IReadOnlyList<IStockQuoteFromDataSource>> GetHistoricalQuotesAsync(Country country, string stockId, DateTime start, DateTime end, Action<Exception> writeToErrorLogAction);
    }
}
