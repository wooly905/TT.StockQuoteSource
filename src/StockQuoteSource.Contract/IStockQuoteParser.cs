using System;
using System.Collections.Generic;

namespace TTStockQuoteSource.Contracts
{
    public interface IStockQuoteParser
    {
        IStockQuoteFromDataSource ParseSingleQuote(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction);
        IReadOnlyList<IStockQuoteFromDataSource> ParseMultiQuotes(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction);
    }
}
