using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource
{
    public abstract class StockDataSourceBase : IStockQuoteDataSource
    {
        protected DateTime? CurrentTime;
        protected IConfiguration Configuration;
        protected IStockQuoteDataSourceOperations Operations;

        protected StockDataSourceBase(IConfiguration configuration, StockQuoteSource source, IStockQuoteDataSourceOperations operations)
        {
            Configuration = configuration;
            Source = source;
            Operations = operations;
        }

        public StockQuoteSource Source { get; }
        public abstract Task<IStockQuoteFromDataSource> GetMostRecentQuoteAsync(Country country, string stockId, Action<Exception> writeToErrorLogAction);
        public abstract Task<IReadOnlyList<IStockQuoteFromDataSource>> GetHistoricalQuotesAsync(Country country, string stockId, DateTime start, DateTime end, Action<Exception> writeToErrorLogAction);

        public async Task<(string, IReadOnlyList<Cookie>)> GetHttpContentAsync(string requestUri, IReadOnlyList<Cookie> cookies = null)
        {
            return await Operations.GetHttpContentAsync(requestUri, cookies).ConfigureAwait(false);
        }
    }
}
