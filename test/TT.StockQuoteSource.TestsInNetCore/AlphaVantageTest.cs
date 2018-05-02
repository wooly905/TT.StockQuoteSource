using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource.AlphaVantage;
using TT.StockQuoteSource.Contracts;
using Xunit;

namespace TT.StockQuoteSource.Tests
{
    public class AlphaVantageTest
    {
        private bool _isWriteToErrorLogActionRan;
        private const string Av5MinsJsonFile = "AlphaVantage-HDV-5min.txt";
        private const string Av1DayJsonFile = "AlphaVantage-HDV-1day.txt";
        private const string Av1DayFullOutputJsonFile = "AlphaVantage-HDV-1day-FullOutput.txt";

        private void WriteToErrorLogAction(Exception ex)
        {
            _isWriteToErrorLogActionRan = true;
        }

        [Theory]
        [InlineData(Av1DayJsonFile)]
        [InlineData(Av1DayFullOutputJsonFile)]
        public void ParseMultiQuotesTest(string jsonFile)
        {
            Country country = Country.USA;
            string stockId = "HDV";
            IStockQuoteParser parser = new AlphaVantageParser();
            string jsonContent = TestUtilities.ReadTestFile(jsonFile);

            IReadOnlyList<IStockQuoteFromDataSource> quotes = parser.ParseMultiQuotes(country, stockId, jsonContent, WriteToErrorLogAction);

            Assert.NotNull(quotes);
            foreach (IStockQuoteFromDataSource quote in quotes)
            {
                Assert.True(quote.IsValid);
            }
        }

        [Fact]
        public void ParseSingleQuoteTest()
        {
            Country country = Country.USA;
            string stockId = "HDV";

            AlphaVantageParser parser = new AlphaVantageParser();

            string jsonContent = TestUtilities.ReadTestFile(Av5MinsJsonFile);
            IStockQuoteFromDataSource quote = parser.ParseSingleQuote(country, stockId, jsonContent, WriteToErrorLogAction);

            Assert.NotNull(quote);
            Assert.True(quote.IsValid);
        }

        [Fact]
        public void ParseSingleQuoteEmptyInputTest()
        {
            Country country = Country.USA;
            string stockId = "HDV";

            AlphaVantageParser parser = new AlphaVantageParser();
            _isWriteToErrorLogActionRan = false;

            IStockQuoteFromDataSource quote = parser.ParseSingleQuote(country, stockId, null, WriteToErrorLogAction);

            Assert.Null(quote);
            Assert.True(_isWriteToErrorLogActionRan);
        }

        [Fact]
        public void ParseMultiQuotesEmptyInputTest()
        {
            Country country = Country.USA;
            string stockId = "HDV";
            _isWriteToErrorLogActionRan = false;

            AlphaVantageParser parser = new AlphaVantageParser();
            IReadOnlyList<IStockQuoteFromDataSource> quotes = parser.ParseMultiQuotes(country, stockId, null, WriteToErrorLogAction);

            Assert.Null(quotes);
            Assert.True(_isWriteToErrorLogActionRan);
        }

        [Fact(Skip = "This is an option test with Internet enabled. Please apply your own api key to test")]
        public void DataSourceRealTimeWithInternetPriceTest()
        {
            Country country = Country.USA;
            IConfiguration config = TestServiceProvider.GetTestConfiguration();
            string stockId = "HDV";
            IStockQuoteDataSourceOperations operations = new StockQuoteDataSourceOperations();
            IStockQuoteParser parser = new AlphaVantageParser();
            AlphaVantageDataSource source = new AlphaVantageDataSource(config, operations, parser);

            IStockQuoteFromDataSource quote = source.GetMostRecentQuoteAsync(country, stockId, WriteToErrorLogAction).Result;

            Assert.True(quote.IsValid);
        }

        // test with Internet enabled
        [Fact(Skip = "This is an option test with Internet enabled. Please apply your own api key to test")]
        public void DataSourceDailyNormalOutputWithInternetTest()
        {
            Country country = Country.USA;
            IConfiguration config = TestServiceProvider.GetTestConfiguration();
            string stockId = "HDV";

            DateTime start = DateTime.Now.AddDays(-30);
            DateTime end = DateTime.Now.AddDays(-15);

            IStockQuoteDataSourceOperations operations = new StockQuoteDataSourceOperations();
            IStockQuoteParser parser = new AlphaVantageParser();
            AlphaVantageDataSource source = new AlphaVantageDataSource(config, operations, parser);

            IReadOnlyList<IStockQuoteFromDataSource> quotes = source.GetHistoricalQuotesAsync(country, stockId, start, end, WriteToErrorLogAction).Result;

            Assert.True(quotes.Count > 5);

            foreach (IStockQuoteFromDataSource quote in quotes)
            {
                Assert.True(quote.IsValid);
            }
        }

        // test with Internet enabled
        [Fact(Skip = "This is an option test with Internet enabled. Please apply your own api key to test")]
        public void DataSourceDailyFullOutputWithInternetTest()
        {
            Country country = Country.USA;
            IConfiguration config = TestServiceProvider.GetTestConfiguration();
            string stockId = "HDV";

            DateTime start = DateTime.Now.AddDays(-600);
            DateTime end = DateTime.Now.AddDays(-300);

            IStockQuoteDataSourceOperations operations = new StockQuoteDataSourceOperations();
            IStockQuoteParser parser = new AlphaVantageParser();
            AlphaVantageDataSource source = new AlphaVantageDataSource(config, operations, parser);
            IReadOnlyList<IStockQuoteFromDataSource> quotes = source.GetHistoricalQuotesAsync(country, stockId, start, end, WriteToErrorLogAction).Result;

            Assert.Equal(207, quotes.Count);

            foreach (IStockQuoteFromDataSource quote in quotes)
            {
                Assert.True(quote.IsValid);
            }
        }
    }
}
