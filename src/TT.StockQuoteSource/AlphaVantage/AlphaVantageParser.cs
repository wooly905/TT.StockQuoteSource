using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TT.StockQuoteSource.Contracts;

namespace TT.StockQuoteSource.AlphaVantage
{
    /// <summary>
    /// class of AlphaVantageParser
    /// </summary>
    public class AlphaVantageParser : IStockQuoteParser
    {
        /// <summary>
        /// Parse single stock quote from Alpha Vantage
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="httpResponseContent"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        public IStockQuoteFromDataSource ParseSingleQuote(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction)
        {
            if (string.IsNullOrEmpty(httpResponseContent))
            {
                writeToErrorLogAction?.Invoke(new Exception("Http Response content is null or empty"));

                return null;
            }

            JObject jObject = JObject.Parse(httpResponseContent);
            JToken dataToken = jObject["Time Series (5min)"];
            bool isDailyData = false;

            if (dataToken == null)
            {
                dataToken = jObject["Time Series (Daily)"];
                isDailyData = true;
            }

            if (dataToken == null)
            {
                return null;
            }

            StockQuoteResult result = new StockQuoteResult
            {
                Country = country,
                StockId = stockId
            };

            try
            {
                JToken[] tokens = dataToken.ToArray();
                result.HighPrice = decimal.MinValue;
                result.LowPrice = decimal.MaxValue;
                DateTime? _time = null;

                for (int i = 0; i < tokens.Length; i++)
                {
                    JProperty jpro = (JProperty)tokens[i];

                    if (!DateTime.TryParse(jpro.Name, out DateTime tradeTime))
                    {
                        return null;
                    }

                    if (isDailyData)
                    {
                        // adjust daily time to 16:00:00 for historical data consideration
                        tradeTime = new DateTime(tradeTime.Year, tradeTime.Month, tradeTime.Day, 16, 00, 0);
                    }

                    _time = tradeTime;

                    if (i == 0)
                    {
                        result.TradeDateTime = _time.Value;
                    }

                    if (!result.TradeDateTime.IsSameDay(_time.Value))
                    {
                        break;
                    }

                    result.OpenPrice = jpro.Value["1. open"].Value<decimal>();
                    // select highest price of a day
                    decimal tempHighPrice = jpro.Value["2. high"].Value<decimal>();
                    if (result.HighPrice < tempHighPrice)
                    {
                        result.HighPrice = tempHighPrice;
                    }
                    // select lowest price of a day
                    decimal tempLowPrice = jpro.Value["3. low"].Value<decimal>();
                    if (result.LowPrice > tempLowPrice)
                    {
                        result.LowPrice = tempLowPrice;
                    }
                    // close price (last price) is in the first row
                    if (i == 0)
                    {
                        result.ClosePrice = jpro.Value["4. close"].Value<decimal>();
                    }
                    // accumulate all volumes of a day
                    result.Volume = result.Volume + jpro.Value["5. volume"].Value<int>();
                }
            }
            catch (Exception ex)
            {
                writeToErrorLogAction?.Invoke(ex);

                return null;
            }

            return result;
        }

        /// <summary>
        /// Parse multiple stock quotes from Alpha Vantage
        /// </summary>
        /// <param name="country"></param>
        /// <param name="stockId"></param>
        /// <param name="httpResponseContent"></param>
        /// <param name="writeToErrorLogAction"></param>
        /// <returns></returns>
        public IReadOnlyList<IStockQuoteFromDataSource> ParseMultiQuotes(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction)
        {
            if (string.IsNullOrEmpty(httpResponseContent))
            {
                writeToErrorLogAction?.Invoke(new Exception("Http response content is emtpy"));
                return null;
            }

            JObject jObject = JObject.Parse(httpResponseContent);
            JToken dataToken = jObject["Time Series (Daily)"];

            if (dataToken == null)
            {
                return null;
            }

            List<IStockQuoteFromDataSource> results = new List<IStockQuoteFromDataSource>();

            try
            {
                JToken[] tokens = dataToken.ToArray();

                foreach (JToken token in tokens)
                {
                    JProperty jpro = (JProperty)token;

                    if (!DateTime.TryParse(jpro.Name, out DateTime tradeTime))
                    {
                        continue;
                    }

                    // adjust daily time to 16:00:00 for historical data consieration
                    DateTime tradeDateTime = new DateTime(tradeTime.Year, tradeTime.Month, tradeTime.Day, 16, 30, 0);

                    decimal openPrice = jpro.Value["1. open"].Value<decimal>();
                    decimal highPrice = jpro.Value["2. high"].Value<decimal>();
                    decimal lowPrice = jpro.Value["3. low"].Value<decimal>();
                    decimal closePrice = jpro.Value["4. close"].Value<decimal>();
                    int volume = jpro.Value["5. volume"].Value<int>();

                    StockQuoteResult quote = new StockQuoteResult
                    {
                        Country = country,
                        StockId = stockId,
                        TradeDateTime = tradeDateTime,
                        OpenPrice = openPrice,
                        HighPrice = highPrice,
                        LowPrice = lowPrice,
                        ClosePrice = closePrice,
                        Volume = volume
                    };

                    results.Add(quote);
                }
            }
            catch (Exception ex)
            {
                writeToErrorLogAction?.Invoke(ex);

                return null;
            }

            return results;
        }
    }
}
