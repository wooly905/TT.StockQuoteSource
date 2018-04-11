using System;
using System.Collections.Generic;

namespace TT.StockQuoteSource.Contracts
{
    /// <summary>
    /// IStockQuoteParser
    /// </summary>
    public interface IStockQuoteParser
    {
        /// <summary>
        /// Parse single stock quote data from data source
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="httpResponseContent"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        IStockQuoteFromDataSource ParseSingleQuote(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction);

        /// <summary>
        /// Parse multiple stock quotes from data source
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="httpResponseContent"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        IReadOnlyList<IStockQuoteFromDataSource> ParseMultiQuotes(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction);
    }
}
