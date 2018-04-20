namespace TT.StockQuoteSource.Contracts
{
    /// <summary>
    /// Stock quote source enum
    /// </summary>
    public enum StockQuoteSource
    {
        /// <summary>
        /// For testing
        /// </summary>
        Test = -1,

        /// <summary>
        /// Yahoo Finance
        /// </summary>
        Yahoo = 0,

        /// <summary>
        /// Alpha Vantage
        /// </summary>
        AlphaVantage = 1,
    }
}
