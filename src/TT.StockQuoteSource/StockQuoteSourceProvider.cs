using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource.AlphaVantage;
using TT.StockQuoteSource.Contracts;
using TT.StockQuoteSource.YahooFinance;

namespace TT.StockQuoteSource
{
    /// <summary>
    /// Class of StockQuoteSourceProvider
    /// </summary>
    public class StockQuoteSourceProvider : IStockQuoteProvider
    {
        private readonly IConfiguration _configuration;
        private readonly Country _country;
        private List<IStockQuoteDataSource> _dataSourceList;
        private readonly IStockQuoteDataSourceOperations _dataSourceOperations;

        /// <summary>
        /// Ctor of StockQuoteSourceProvider
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="country"></param>
        public StockQuoteSourceProvider(IConfiguration configuration, Country country)
        {
            _configuration = configuration;
            _country = country;
            _dataSourceOperations = new StockQuoteDataSourceOperations();
        }

        /// <summary>
        /// Get a list of stock data source
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<IStockQuoteDataSource> GetStockDataSources()
        {
            if (_dataSourceList == null)
            {
                _dataSourceList = new List<IStockQuoteDataSource>();

                // Yahoo is common for now
                IStockQuoteDataSource yahooDataSource = CreateStockSource(Contracts.StockQuoteSource.Yahoo);

                if (yahooDataSource != null)
                {
                    _dataSourceList.Add(yahooDataSource);
                }

                switch (_country)
                {
                    case Country.Taiwan:
                        // TODO : add later
                        break;

                    case Country.USA:
                        IStockQuoteDataSource alphaVantageSource = CreateStockSource(Contracts.StockQuoteSource.AlphaVantage);

                        if (alphaVantageSource != null)
                        {
                            _dataSourceList.Add(CreateStockSource(Contracts.StockQuoteSource.AlphaVantage));
                        }

                        break;

                    case Country.Test:
                        // There is no data source for Test
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return _dataSourceList;
        }

        private IStockQuoteDataSource CreateStockSource(Contracts.StockQuoteSource sourceKind)
        {
            switch (sourceKind)
            {
                case Contracts.StockQuoteSource.Yahoo:
                    IStockQuoteParser yahooParser = new YahooFinanceParser();
                    return new YahooFinanceDataSource(_configuration, _dataSourceOperations, yahooParser);

                case Contracts.StockQuoteSource.AlphaVantage:
                    IStockQuoteParser alphaVantageParser = new AlphaVantageParser();
                    return new AlphaVantageDataSource(_configuration, _dataSourceOperations, alphaVantageParser);

                case Contracts.StockQuoteSource.Test:
                    // There is no test data source required
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceKind), sourceKind, null);
            }

            return null;
        }
    }
}
