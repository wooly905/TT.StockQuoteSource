using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace TTStockQuoteSource.Contracts
{
    public interface IStockQuoteDataSourceOperations
    {
        Task<(string, IReadOnlyList<Cookie>)> GetHttpContentAsync(string requestUri, IReadOnlyList<Cookie> cookies);
    }
}
