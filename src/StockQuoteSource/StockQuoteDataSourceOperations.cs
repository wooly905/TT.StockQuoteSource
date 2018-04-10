using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TTStockQuoteSource.Contracts;

namespace TTStockQuoteSource
{
    public class StockQuoteDataSourceOperations : IStockQuoteDataSourceOperations
    {
        public async Task<(string, IReadOnlyList<Cookie>)> GetHttpContentAsync(string requestUrl, IReadOnlyList<Cookie> cookies = null)
        {
            CookieContainer cookieContainer = new CookieContainer();
            HttpResponseMessage response = await CreateHttpClient(cookieContainer, cookies).GetAsync(requestUrl).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string httpResponseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Uri uri = new Uri(requestUrl);
                IReadOnlyList<Cookie> responseCookies = cookieContainer.GetCookies(uri).Cast<Cookie>().ToList();
                return (httpResponseContent, responseCookies);
            }

            return (null, null);
        }

        private static HttpClient CreateHttpClient(CookieContainer cookieContainer, IReadOnlyList<Cookie> cookies)
        {
            // TODO : change http client here.
            if (cookies != null)
            {
                foreach (Cookie cookie in cookies)
                {
                    cookieContainer.Add(cookie);
                }
            }

            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };

            return new HttpClient(handler);
        }
    }
}
