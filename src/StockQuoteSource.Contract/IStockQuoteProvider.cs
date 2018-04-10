using System.Collections.Generic;

namespace TTStockQuoteSource.Contracts
{
    public interface IStockQuoteProvider
    {
        IReadOnlyList<IStockQuoteDataSource> CreateStockDataSource();
    }
}
