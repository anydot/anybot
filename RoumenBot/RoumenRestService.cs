using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoumenBot
{
    public class RoumenRestService<T> : IRoumenRestService<T> where T : ITag, new()
    {
        private readonly IOptions<RoumenOptions<T>> options;
        private readonly HttpClient httpClient;
        private readonly IRoumenParser roumenParser;
        private readonly IRoumenResponseLogger<T> roumenResponseLogger;

        public RoumenRestService(HttpClient httpClient, IOptions<RoumenOptions<T>> options, IRoumenParser roumenParser, IRoumenResponseLogger<T> roumenResponseLogger)
        {
            this.options = options;
            this.httpClient = httpClient;
            this.roumenParser = roumenParser;
            this.roumenResponseLogger = roumenResponseLogger;
        }

        public async Task<IEnumerable<RoumenImage<T>>> FetchImagesFromWeb()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, options.Value.DataUrl);
            var baseUrl = new Uri(options.Value.DataUrl!).GetLeftPart(UriPartial.Authority);

            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("x-requested-with", "xhr");
            using var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            roumenResponseLogger.LogResponse(responseString);

            return roumenParser.Parse<T>(responseString, baseUrl);
        }
    }
}