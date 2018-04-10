using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource.YahooFinance
{
    public class YahooFinanceDataSource : StockDataSourceBase
    {
        private readonly IStockQuoteParser _parser;

        public YahooFinanceDataSource(IConfiguration configuration, IStockQuoteDataSourceOperations operations, IStockQuoteParser parser)
            : base(configuration, StockQuoteSource.Yahoo, operations)
        {
            _parser = parser;
        }

        public override async Task<IStockQuoteFromDataSource> GetMostRecentQuoteAsync(Country country, string stockId, Action<Exception> writeToErrorLogAction)
        {
            string stockFullId = country == Country.USA ? stockId : $"{stockId}.{country.GetShortName()}";
            string yahooUrl = string.Format(Configuration["YahooFinanceURL"], stockFullId);
            (string htmlContent, IReadOnlyList<Cookie> cookies) response = await GetHttpContentAsync(yahooUrl).ConfigureAwait(false);

            return _parser.ParseSingleQuote(country, stockId, response.htmlContent, writeToErrorLogAction);
        }

        public override async Task<IReadOnlyList<IStockQuoteFromDataSource>> GetHistoricalQuotesAsync(Country country, string stockId, DateTime start, DateTime end, Action<Exception> writeToErrorLogAction)
        {
            string stockFullId = country == Country.USA ? stockId : $"{stockId}.{country.GetShortName()}";
            string yahooSingleQuoteUrl = string.Format(Configuration["YahooFinanceURL"], stockFullId);
            (string htmlContent, IReadOnlyList<Cookie> cookies) response = await GetHttpContentAsync(yahooSingleQuoteUrl).ConfigureAwait(false);
            IStockQuoteFromDataSource yahooQuote = _parser.ParseSingleQuote(country, stockId, response.htmlContent, writeToErrorLogAction);
            IReadOnlyList<Cookie> cookies = response.cookies;

            if (!(yahooQuote is YahooFinanceDataResult yahooResult) || string.IsNullOrEmpty(yahooResult.Crumb))
            {
                return null;
            }

            string startTimestamp = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0).ToUnixTimestamp();
            string endTimestamp = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59).ToUnixTimestamp();
            string yahooHistoricalUrl = string.Format(Configuration["YahooHistoricalDataUrl"], stockFullId, startTimestamp, endTimestamp, yahooResult.Crumb);
            (string htmlContent, IReadOnlyList<Cookie> cookies) response2 = await GetHttpContentAsync(yahooHistoricalUrl, cookies).ConfigureAwait(false);

            IReadOnlyList<IStockQuoteFromDataSource> quotes = _parser.ParseMultiQuotes(country, stockId, response2.htmlContent, writeToErrorLogAction);

            return quotes.Where(a => a.TradeDateTime >= start && a.TradeDateTime <= end).ToList();
        }
    }
}
