using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TT.StockQuoteSource.Contracts
{
    /// <summary>
    /// IStockQuoteDataSource
    /// </summary>
    public interface IStockQuoteDataSource
    {
        /// <summary>
        /// Stock quote source
        /// </summary>
        StockQuoteSource Source { get; }

        /// <summary>
        /// Get the most recent stock quote
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        Task<IStockQuoteFromDataSource> GetMostRecentQuoteAsync(Country country, string stockId, Action<Exception> writeToErrorLogAction);

        /// <summary>
        /// Get historical stock quotes
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        Task<IReadOnlyList<IStockQuoteFromDataSource>> GetHistoricalQuotesAsync(Country country, string stockId, DateTime start, DateTime end, Action<Exception> writeToErrorLogAction);
    }
}
