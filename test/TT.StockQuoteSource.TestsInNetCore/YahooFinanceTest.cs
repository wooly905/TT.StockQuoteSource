using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource.Contracts;
using TT.StockQuoteSource.YahooFinance;
using Xunit;

namespace TT.StockQuoteSource.Tests
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

            Assert.NotNull(quote);
            Assert.True(quote.IsValid);
            Assert.Equal(14.63m, quote.ClosePrice);
            Assert.Equal(15.0686m, quote.HighPrice);
            Assert.Equal(14.611m, quote.LowPrice);
            Assert.Equal(14.79m, quote.OpenPrice);
            Assert.Equal(67165, quote.Volume);
            Assert.Equal(stockId, quote.StockId);
            Assert.Equal(country, quote.Country);
            Assert.Equal(2018, quote.TradeDateTime.Year);
            Assert.Equal(3, quote.TradeDateTime.Month);
            Assert.Equal(2, quote.TradeDateTime.Day);
            Assert.Equal(15, quote.TradeDateTime.Hour);
            Assert.Equal(59, quote.TradeDateTime.Minute);
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

        [Theory]
        [InlineData(Country.USA, "HDV")]
        [InlineData(Country.Taiwan, "0050")]
        [InlineData(Country.HK, "0388")]
        public void GetMostRecentQuoteTest(Country country, string stockId)
        {
            IConfiguration config = TestServiceProvider.GetTestConfiguration();
            IStockQuoteProvider provider = new StockQuoteSourceProvider(config, country);
            IStockQuoteDataSource yahooSource = provider.GetStockDataSources().FirstOrDefault(a => a.Source == Contracts.StockQuoteSource.Yahoo);

            Assert.NotNull(yahooSource);

            IStockQuoteFromDataSource result = yahooSource.GetMostRecentQuoteAsync(country, stockId, WriteToErrorLogAction).Result;

            Assert.NotNull(result);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(Country.USA, "HDV")]
        [InlineData(Country.Taiwan, "0050")]
        [InlineData(Country.HK, "0388")]
        public void GetHistoricalQuotesTest(Country country, string stockId)
        {
            IConfiguration config = TestServiceProvider.GetTestConfiguration();
            IStockQuoteProvider provider = new StockQuoteSourceProvider(config, country);
            IStockQuoteDataSource yahooSource = provider.GetStockDataSources().FirstOrDefault(a => a.Source == Contracts.StockQuoteSource.Yahoo);

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
