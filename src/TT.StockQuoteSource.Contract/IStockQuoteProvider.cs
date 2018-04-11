using System.Collections.Generic;

namespace TT.StockQuoteSource.Contracts
{
    /// <summary>
    /// IStockQuoteProvider
    /// </summary>
    public interface IStockQuoteProvider
    {
        /// <summary>
        /// Get a list of stock data source
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IStockQuoteDataSource> GetStockDataSources();
    }
}
