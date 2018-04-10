using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource.YahooFinance
{
    public class YahooFinanceParser : IStockQuoteParser
    {
        private const string _htmlJsonContentStartKeyword = "{\"context\":{";
        private const string _htmlJsonContentEndKeyword = "}(this));";
        private const string _errorMessagePrefix = "Parsing Yahoo HTML";

        public IStockQuoteFromDataSource ParseSingleQuote(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction)
        {
            int jsonStartPosition = httpResponseContent.IndexOf(_htmlJsonContentStartKeyword, StringComparison.OrdinalIgnoreCase);
            if (jsonStartPosition <= 0)
            {
                string message = $"{_errorMessagePrefix} ({stockId}) start position cannot be found.";
                writeToErrorLogAction?.Invoke(new Exception(message));
                return null;
            }

            int jsonEndPosition = httpResponseContent.IndexOf(_htmlJsonContentEndKeyword, StringComparison.OrdinalIgnoreCase);
            if (jsonEndPosition <= 0)
            {
                string message = $"{_errorMessagePrefix} ({stockId}) end position cannot be found.";
                writeToErrorLogAction?.Invoke(new Exception(message));
                return null;
            }

            if (jsonStartPosition >= jsonEndPosition)
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on start position >= end position.";
                writeToErrorLogAction?.Invoke(new Exception(message));
                return null;
            }

            string targetJson = httpResponseContent.Substring(jsonStartPosition, jsonEndPosition - 2 - jsonStartPosition);
            StringBuilder sb = new StringBuilder(targetJson);

            // Yahoo online and test file have difference. weird situation.
            // an workaround to remove the trailing semicolon
            if (targetJson.EndsWith(";"))
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return SingleQuoteParseInternal(country, stockId, sb.ToString(), writeToErrorLogAction);
        }

        private IStockQuoteFromDataSource SingleQuoteParseInternal(Country country, string stockId, string targetJsonString, Action<Exception> writeToErrorLogAction)
        {
            JObject jObject = JObject.Parse(targetJsonString);
            JToken priceToken;

            try
            {
                priceToken = jObject["context"]?["dispatcher"]?["stores"]?["QuoteSummaryStore"]?["price"];
                if (priceToken == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting price element under QuoteSummaryStore";
                writeToErrorLogAction?.Invoke(new Exception(message));

                return null;
            }

            // open price
            decimal openPrice;
            try
            {
                JToken openPriceToken = priceToken["regularMarketOpen"]?["raw"];
                if (openPriceToken == null)
                {
                    throw new Exception();
                }

                openPrice = openPriceToken.Value<decimal>();
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting open price element (regularMarketOpen)";
                writeToErrorLogAction?.Invoke(new Exception(message));

                return null;
            }

            // time
            DateTime tradeDateTime;
            try
            {
                JToken marketTimeToken = priceToken["regularMarketTime"];
                if (marketTimeToken == null)
                {
                    throw new Exception();
                }

                int unixTimeStamp = marketTimeToken.Value<int>();
                tradeDateTime = new DateTime(1970, 1, 1).AddSeconds(unixTimeStamp);
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting time element (regularMarketTime)";
                writeToErrorLogAction?.Invoke(new Exception(message));

                return null;
            }


            // high price
            decimal highPrice;
            try
            {
                JToken highPriceToken = priceToken["regularMarketDayHigh"]?["raw"];
                if (highPriceToken == null)
                {
                    throw new Exception();
                }

                highPrice = highPriceToken.Value<decimal>();
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting high price element (regularMarketDayHigh)";
                writeToErrorLogAction?.Invoke(new Exception(message));

                return null;
            }

            // low price
            decimal lowPrice;
            try
            {
                JToken lowPriceToken = priceToken["regularMarketDayLow"]?["raw"];
                if (lowPriceToken == null)
                {
                    throw new Exception();
                }

                lowPrice = lowPriceToken.Value<decimal>();
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting low price element (regularMarketDayLow)";
                writeToErrorLogAction?.Invoke(new Exception(message));

                return null;
            }

            // current price ( market price or close price)
            decimal closePrice;
            try
            {
                JToken currentPriceToken = priceToken["regularMarketPrice"]?["raw"];
                if (currentPriceToken == null)
                {
                    throw new Exception();
                }

                closePrice = currentPriceToken.Value<decimal>();
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting current/close price element (regularMarketPrice)";
                writeToErrorLogAction?.Invoke(new Exception(message));

                return null;
            }

            // volume
            int volume;
            try
            {
                JToken volumeToken = priceToken["regularMarketVolume"]?["raw"];
                if (volumeToken == null)
                {
                    throw new Exception();
                }

                volume = volumeToken.Value<int>();
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting volume element (regularMarketVolume)";
                writeToErrorLogAction?.Invoke(new Exception(message));

                return null;
            }

            // crumb
            string crumb = string.Empty;
            try
            {
                JToken crumbToken = jObject["context"]?["dispatcher"]?["stores"]?["CrumbStore"]?["crumb"];
                if (crumbToken == null)
                {
                    throw new Exception();
                }

                crumb = crumbToken.Value<string>();
            }
            catch
            {
                string message = $"{_errorMessagePrefix} ({stockId}) error on getting crumb element (crumb)";
                writeToErrorLogAction?.Invoke(new Exception(message));
            }

            YahooFinanceDataResult yahooResult = new YahooFinanceDataResult(country, stockId, tradeDateTime, lowPrice, highPrice, closePrice, openPrice, volume)
            {
                Crumb = crumb
            };

            return yahooResult;
        }

        public IReadOnlyList<IStockQuoteFromDataSource> ParseMultiQuotes(Country country, string stockId, string httpResponseContent, Action<Exception> writeToErrorLogAction)
        {
            string[] lines = httpResponseContent?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines == null || lines.Length == 0)
            {
                return null;
            }

            List<IStockQuoteFromDataSource> result = new List<IStockQuoteFromDataSource>();

            // make first line go away
            for (int count = 1; count < lines.Length; count++)
            {
                string[] content = lines[count].Split(',');
                if (content.Length != 7)
                {
                    writeToErrorLogAction?.Invoke(new Exception($"{country} {stockId} - one record is less than 7 - from Yahoo data sourceKind"));
                    continue;
                }

                if (!DateTime.TryParse(content[0], out DateTime tradeDate))
                {
                    writeToErrorLogAction?.Invoke(new Exception($"{country} {stockId} - one record is failed on parsing date ({content[0]}) - from Yahoo data sourceKind"));
                    continue;
                }

                if (!decimal.TryParse(content[1], out decimal openPrice))
                {
                    writeToErrorLogAction?.Invoke(new Exception($"{country} {stockId} - one record is failed on parsing open price ({content[0]} - {content[1]}) - from Yahoo data sourceKind"));
                    continue;
                }

                if (!decimal.TryParse(content[2], out decimal highPrice))
                {
                    writeToErrorLogAction?.Invoke(new Exception($"{country} {stockId} - one record is failed on parsing high price ({content[0]} - {content[2]}) - from Yahoo data sourceKind"));
                    continue;
                }

                if (!decimal.TryParse(content[3], out decimal lowPrice))
                {
                    writeToErrorLogAction?.Invoke(new Exception($"{country} {stockId} - one record is failed on parsing low price ({content[0]} - {content[3]}) - from Yahoo data sourceKind"));
                    continue;
                }

                if (!decimal.TryParse(content[4], out decimal closePrice))
                {
                    writeToErrorLogAction?.Invoke(new Exception($"{country} {stockId} - one record is failed on parsing close price ({content[0]} - {content[4]}) - from Yahoo data sourceKind"));
                    continue;
                }

                if (!int.TryParse(content[6], out int volume))
                {
                    writeToErrorLogAction?.Invoke(new Exception($"{country} {stockId} - one record is failed on parsing volume ({content[0]} - {content[6]}) - from Yahoo data sourceKind"));
                    continue;
                }

                result.Add(new YahooFinanceDataResult(country, stockId, tradeDate, lowPrice, highPrice, closePrice, openPrice, volume));
            }

            return result;
        }
    }
}
