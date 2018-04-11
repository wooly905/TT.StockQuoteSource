using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace TT.StockQuoteSource.Contracts
{
    /// <summary>
    /// IStockQuoteDataSourceOperations
    /// </summary>
    public interface IStockQuoteDataSourceOperations
    {
        /// <summary>
        /// Get http response content as string and associated cookies
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        Task<(string, IReadOnlyList<Cookie>)> GetHttpContentAsync(string requestUri, IReadOnlyList<Cookie> cookies);
    }
}
