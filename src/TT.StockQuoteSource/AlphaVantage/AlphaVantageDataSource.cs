using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource.Contracts;

namespace TT.StockQuoteSource.AlphaVantage
{
    // Don't use Alpha Vantage for Taiwan stocks because it doesn't support all stocks.
    // for example, 0050 is supported and 0056 isn't.

    /// <summary>
    /// class of AlphaVantageDataSource
    /// </summary>
    public class AlphaVantageDataSource : StockDataSourceBase
    {
        private readonly IStockQuoteParser _parser;

        /// <summary>
        /// ctor of AlphaVantageDataSource
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="operations"></param>
        /// <param name="parser"></param>
        public AlphaVantageDataSource(IConfiguration configuration, IStockQuoteDataSourceOperations operations, IStockQuoteParser parser)
          : base(configuration, Contracts.StockQuoteSource.AlphaVantage, operations)
        {
            _parser = parser;
        }

        /// <summary>
        /// Get the most recent stock quote from Alpha Vantage
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        public override async Task<IStockQuoteFromDataSource> GetMostRecentQuoteAsync(Country country, string stockId, Action<Exception> writeToErrorLogAction)
        {
            string stockFullId = country == Country.USA ? stockId : $"{stockId}.{country.GetShortName()}";
            string avUrl = Configuration["AlphaVantageRealTimeUrl"];
            string key = Configuration["AlphaVantageKey"];
            string url = string.Format(avUrl, stockFullId, key);
            (string JsonContent, IReadOnlyList<Cookie> Cookies) response = await GetHttpContentAsync(url).ConfigureAwait(false);

            return _parser.ParseSingleQuote(country, stockId, response.JsonContent, writeToErrorLogAction);
        }

        /// <summary>
        /// Get historical stock quotes from Alpha Vantage
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
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
            (string JsonContent, IReadOnlyList<Cookie> Cookies) response = await GetHttpContentAsync(string.Format(avUrl, stockFullId, key)).ConfigureAwait(false);

            IReadOnlyList<IStockQuoteFromDataSource> quotes = _parser.ParseMultiQuotes(country, stockId, response.JsonContent, writeToErrorLogAction);

            return quotes.Where(a => a.TradeDateTime >= start && a.TradeDateTime <= end).ToList();
        }
    }
}
