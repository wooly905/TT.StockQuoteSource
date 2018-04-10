using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource.AlphaVantage
{
    // Don't use Alpha Vantage for Taiwan stocks because it doesn't support all stocks.
    // for example, 0050 is supported and 0056 isn't.

    public class AlphaVantageDataSource : StockDataSourceBase
    {
        private readonly IStockQuoteParser _parser;

        public AlphaVantageDataSource(IConfiguration configuration, IStockQuoteDataSourceOperations operations, IStockQuoteParser parser)
          : base(configuration, StockQuoteSource.AlphaVantage, operations)
        {
            _parser = parser;
        }

        public override async Task<IStockQuoteFromDataSource> GetMostRecentQuoteAsync(Country country, string stockId, Action<Exception> writeToErrorLogAction)
        {
            string stockFullId = country == Country.USA ? stockId : $"{stockId}.{country.GetShortName()}";
            string avUrl = Configuration["AlphaVantageRealTimeUrl"];
            string key = Configuration["AlphaVantageKey"];
            string url = string.Format(avUrl, stockFullId, key);
            (string jsonContent, IReadOnlyList<Cookie> cookies) response = await GetHttpContentAsync(url).ConfigureAwait(false);

            return _parser.ParseSingleQuote(country, stockId, response.jsonContent, writeToErrorLogAction);
        }

        public override async Task<IReadOnlyList<IStockQuoteFromDataSource>> GetHistoricalQuotesAsync(Country country, string stockId, DateTime start, DateTime end, Action<Exception> writeToErrorLogAction)
        {
            // if (end - start > 4 months, go to full output)
            string stockFullId = country == Country.USA ? stockId : $"{stockId}.{country.GetShortName()}";
            string avUrl = Configuration["AlphaVantageDailyUrl"];

            if ((end - start).TotalDays > 120)
            {
                avUrl = Configuration["AlphaVantageDailyFullOutputUrl"];
            }

            string key = Configuration["AlphaVantageKey"];
            (string jsonContent, IReadOnlyList<Cookie> cookies) response = await GetHttpContentAsync(string.Format(avUrl, stockFullId, key)).ConfigureAwait(false);

            IReadOnlyList<IStockQuoteFromDataSource> quotes = _parser.ParseMultiQuotes(country, stockId, response.jsonContent, writeToErrorLogAction);

            return quotes.Where(a => a.TradeDateTime >= start && a.TradeDateTime <= end).ToList();
        }
    }
}
