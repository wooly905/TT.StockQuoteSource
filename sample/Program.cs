using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TT.StockQuoteSource;
using TT.StockQuoteSource.Contracts;

namespace SampleApp
{
    class Program
    {
        private static IStockQuoteProvider _provider;
        private static IConfiguration _config;
        private static Country _country;

        static async Task Main(string[] args)
        {
            _country = Country.USA;
            _config = GetConfiguration();
            _provider = new StockQuoteSourceProvider(_config, _country);

            Console.WriteLine("Yahoo Finance:");
            await RunYahooSource();

            Console.WriteLine("Alpha Vantage Finance:");
            await RunAlphaVantageSource();

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        static IConfiguration GetConfiguration()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("StockQuoteSourceConfig.json");
            return configBuilder.Build();
        }

        static IStockQuoteDataSource GetYahooDataSource(IStockQuoteProvider provider)
        {
            return provider.GetStockDataSources().FirstOrDefault(a => a.Source == StockQuoteSource.Yahoo);
        }

        static IStockQuoteDataSource GetAlphaVantageDataSource(IStockQuoteProvider provider)
        {
            return provider.GetStockDataSources().FirstOrDefault(a => a.Source == StockQuoteSource.AlphaVantage);
        }

        static async Task RunYahooSource()
        {
            string stockId = "HDV";
            IStockQuoteDataSource yahooDataSource = GetYahooDataSource(_provider);

            if (yahooDataSource == null)
            {
                Console.WriteLine("Error : Yahoo data source object is null");
                return;
            }

            IStockQuoteFromDataSource quote = await yahooDataSource.GetMostRecentQuoteAsync(_country, stockId, WriteToError).ConfigureAwait(false);

            if (quote != null)
            {
                PrintQuote(quote);
            }
        }

        static async Task RunAlphaVantageSource()
        {
            string stockId = "HDV";
            IStockQuoteDataSource alphaVantageDataSource = GetAlphaVantageDataSource(_provider);

            if (alphaVantageDataSource == null)
            {
                Console.WriteLine("Error : Yahoo data source object is null");
                return;
            }

            IStockQuoteFromDataSource quote = await alphaVantageDataSource.GetMostRecentQuoteAsync(_country, stockId, WriteToError).ConfigureAwait(false);

            if (quote != null)
            {
                PrintQuote(quote);
            }
        }

        static void PrintQuote(IStockQuoteFromDataSource quote)
        {
            if (quote == null)
            {
                return;
            }

            Console.WriteLine("Date: " + quote.TradeDateTime);
            Console.WriteLine("Open: " + quote.OpenPrice);
            Console.WriteLine("Close: " + quote.ClosePrice);
            Console.WriteLine("High: " + quote.HighPrice);
            Console.WriteLine("Low: " + quote.LowPrice);
            Console.WriteLine("Volume: " + quote.Volume);
            Console.WriteLine();
        }

        static void WriteToError(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }
        }
    }
}
