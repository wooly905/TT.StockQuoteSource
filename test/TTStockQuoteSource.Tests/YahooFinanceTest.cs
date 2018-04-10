using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TTStockQuoteSource;
using TTStockQuoteSource.Contracts;
using TTStockQuoteSource.YahooFinance;
using Xunit;

namespace TTStockSource.Tests
{
    public class YahooFinanceTest
    {
        private bool _isWriteToErrorLogActionRan;
        private const string YahooHtmlTestFilename = "YahooHtml-DRV.txt";
        private const string YahooMultiQuotesFilename = "YahooHistoricalData-2002.csv";

        private void WriteToErrorLogAction(Exception ex)
        {
            _isWriteToErrorLogActionRan = true;
        }

        [Fact]
        public void ParseSingleQuoteTest()
        {
            Country country = Country.USA;
            string stockId = "DRV";
            string htmlContent = TestUtilities.ReadTestFile(YahooHtmlTestFilename);
            IStockQuoteParser parser = new YahooFinanceParser();

            IStockQuoteFromDataSource quote = parser.ParseSingleQuote(country, stockId, htmlContent, WriteToErrorLogAction);

            Assert.True(quote.IsValid);
        }

        [Fact]
        public void ParseMultiQuotesTest()
        {
            Country country = Country.Taiwan;
            string stockId = "2002";
            string htmlContent = TestUtilities.ReadTestFile(YahooMultiQuotesFilename);
            IStockQuoteParser parser = new YahooFinanceParser();

            IReadOnlyList<IStockQuoteFromDataSource> quotes = parser.ParseMultiQuotes(country, stockId, htmlContent, WriteToErrorLogAction);

            Assert.NotNull(quotes);
            foreach (IStockQuoteFromDataSource quote in quotes)
            {
                Assert.True(quote.IsValid);
            }
        }

        // run with Internet enabled
        [Fact]
        public void DataSourceUsaSingleResultTest()
        {
            Country country = Country.USA;
            string stockId = "HDV";
            DataSourceUsaSingleResultInternal(country, stockId);
        }

        // run with Internet enabled
        [Fact]
        public void DataSourceTaiwanSingleResultTest()
        {
            Country country = Country.Taiwan;
            string stockId = "0056";
            DataSourceUsaSingleResultInternal(country, stockId);
        }

        private void DataSourceUsaSingleResultInternal(Country country, string stockId)
        {
            IConfiguration config = TestServiceProvider.GetTestConfiguration();
            IStockQuoteProvider provider = new StockQuoteSourceProvider(config, country);
            IStockQuoteDataSource yahooSource = provider.CreateStockDataSource().FirstOrDefault(a => a.Source == StockQuoteSource.Yahoo);

            Assert.NotNull(yahooSource);

            IStockQuoteFromDataSource result = yahooSource.GetMostRecentQuoteAsync(country, stockId, WriteToErrorLogAction).Result;

            Assert.NotNull(result);
            Assert.True(result.IsValid);
        }

        // run with Internet enabled
        [Fact]
        public void DataSourceMultipleResultUsaDataTest()
        {
            Country country = Country.USA;
            string stockId = "HDV";
            DataSourceMultipleResultInternal(country, stockId);
        }

        // run with Internet enabled
        [Fact]
        public void DataSourceMultipleResultTaiwanDataTest()
        {
            Country country = Country.Taiwan;
            string stockId = "2002";
            DataSourceMultipleResultInternal(country, stockId);
        }

        private void DataSourceMultipleResultInternal(Country country, string stockId)
        {
            IConfiguration config = TestServiceProvider.GetTestConfiguration();
            IStockQuoteProvider provider = new StockQuoteSourceProvider(config, country);
            IStockQuoteDataSource yahooSource = provider.CreateStockDataSource().FirstOrDefault(a => a.Source == StockQuoteSource.Yahoo);

            Assert.NotNull(yahooSource);

            DateTime start = new DateTime(2018, 3, 12);
            DateTime end = new DateTime(2018, 3, 16);

            IReadOnlyList<IStockQuoteFromDataSource> results = yahooSource.GetHistoricalQuotesAsync(country, stockId, start, end, WriteToErrorLogAction).Result;

            Assert.NotNull(results);
            Assert.Equal(5, results.Count);

            foreach (IStockQuoteFromDataSource data in results)
            {
                Assert.True(data.IsValid);
            }
        }
    }
}
