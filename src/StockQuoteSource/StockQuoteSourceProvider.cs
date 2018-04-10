using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TTStockQuoteSource.AlphaVantage;
using TTStockQuoteSource.Contracts;
using TTStockQuoteSource.YahooFinance;

namespace TTStockQuoteSource
{
    public class StockQuoteSourceProvider : IStockQuoteProvider
    {
        private readonly IConfiguration _configuration;
        private readonly Country _country;

        public StockQuoteSourceProvider(IConfiguration configuration, Country country)
        {
            _configuration = configuration;
            _country = country;
        }

        public IReadOnlyList<IStockQuoteDataSource> CreateStockDataSource()
        {
            List<IStockQuoteDataSource> result = new List<IStockQuoteDataSource>();

            // Yahoo is common for now
            IStockQuoteDataSource yahooDataSource = CreateStockSource(StockQuoteSource.Yahoo);

            if (yahooDataSource != null)
            {
                result.Add(yahooDataSource);
            }

            switch (_country)
            {
                case Country.Taiwan:
                    // TODO : add later
                    break;
                case Country.USA:
                    IStockQuoteDataSource alphaVantageSource = CreateStockSource(StockQuoteSource.AlphaVantage);
                    if (alphaVantageSource != null)
                    {
                        result.Add(CreateStockSource(StockQuoteSource.AlphaVantage));
                    }
                    break;
            }

            return result;
        }

        private IStockQuoteDataSource CreateStockSource(StockQuoteSource sourceKind)
        {
            IStockQuoteDataSourceOperations operations = new StockQuoteDataSourceOperations();

            switch (sourceKind)
            {
                case StockQuoteSource.Yahoo:
                    IStockQuoteParser yahooParser = new YahooFinanceParser();
                    return new YahooFinanceDataSource(_configuration, operations, yahooParser);
                case StockQuoteSource.AlphaVantage:
                    IStockQuoteParser alphaVantageParser = new AlphaVantageParser();
                    return new AlphaVantageDataSource(_configuration, operations, alphaVantageParser);
            }

            return null;
        }
    }
}
