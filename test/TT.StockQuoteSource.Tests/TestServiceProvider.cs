using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TT.StockQuoteSource.Tests
{
    internal class TestServiceProvider
    {
        public static IConfiguration GetTestConfiguration()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
            IConfigurationBuilder configBuilder = new ConfigurationBuilder().SetBasePath(filePath)
                                                                            .AddJsonFile("StockQuoteSourceConfig.json");

            return configBuilder.Build();
        }
    }
}
