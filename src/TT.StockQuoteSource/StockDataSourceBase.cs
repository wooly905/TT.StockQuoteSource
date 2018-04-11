using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource.Contracts;

namespace TT.StockQuoteSource
{
    /// <summary>
    /// Class of StockDataSourceBase
    /// </summary>
    public abstract class StockDataSourceBase : IStockQuoteDataSource
    {
        /// <summary>
        /// Current time
        /// </summary>
        protected DateTime? CurrentTime;

        /// <summary>
        /// Configuration
        /// </summary>
        protected IConfiguration Configuration;

        /// <summary>
        /// Stock data source operation
        /// </summary>
        protected IStockQuoteDataSourceOperations Operations;

        /// <summary>
        /// ctor of StockDataSourceBase
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="source"></param>
        /// <param name="operations"></param>
        protected StockDataSourceBase(IConfiguration configuration, Contracts.StockQuoteSource source, IStockQuoteDataSourceOperations operations)
        {
            Configuration = configuration;
            Source = source;
            Operations = operations;
        }

        /// <summary>
        /// Get stock quote source
        /// </summary>
        public Contracts.StockQuoteSource Source { get; }

        /// <summary>
        /// Get the most recent stock quote. This will be real time quote while the market is open.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        public abstract Task<IStockQuoteFromDataSource> GetMostRecentQuoteAsync(Country country, string stockId, Action<Exception> writeToErrorLogAction);

        /// <summary>
        /// Get historical stock quote between times.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        public abstract Task<IReadOnlyList<IStockQuoteFromDataSource>> GetHistoricalQuotesAsync(Country country, string stockId, DateTime start, DateTime end, Action<Exception> writeToErrorLogAction);

        /// <summary>
        /// Get http response content as string and associated cookies.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public async Task<(string, IReadOnlyList<Cookie>)> GetHttpContentAsync(string requestUri, IReadOnlyList<Cookie> cookies = null)
        {
            return await Operations.GetHttpContentAsync(requestUri, cookies).ConfigureAwait(false);
        }
    }
}
