using System;
using TT.StockQuoteSource.Contracts;
using Xunit;

namespace TT.StockQuoteSource.Tests
{
    public class StockQuoteResultTest
    {
        [Fact]
        public void DataResultFromDataTest()
        {
            decimal openPrice = 84.33m;
            decimal highPrice = 84.8m;
            decimal lowPrice = 84.16m;
            decimal closePrice = 84.48m;
            int volume = 417313;

            Country country = Country.Test;
            string testStockId = "HDV";
            DateTime tradeTime = DateTime.Now;

            StockQuoteResult quote = new StockQuoteResult(country, testStockId, tradeTime, lowPrice, highPrice, closePrice, openPrice, volume);

            Assert.Equal(country, quote.Country);
            Assert.Equal(testStockId, quote.StockId);
            Assert.Equal(tradeTime, quote.TradeDateTime);
            Assert.Equal(openPrice, quote.OpenPrice);
            Assert.Equal(highPrice, quote.HighPrice);
            Assert.Equal(lowPrice, quote.LowPrice);
            Assert.Equal(closePrice, quote.ClosePrice);
            Assert.Equal(volume, quote.Volume);
            Assert.True(quote.IsValid);
        }
    }
}
