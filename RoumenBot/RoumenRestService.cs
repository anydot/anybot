using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoumenBot
{
    public class RoumenRestService : IRoumenRestService
    {
        private readonly IOptions<RoumenOptions> options;
        private readonly HttpClient httpClient;
        private readonly IRoumenParser roumenParser;

        public RoumenRestService(HttpClient httpClient, IOptions<RoumenOptions> options, IRoumenParser roumenParser)
        {
            this.options = options;
            this.httpClient = httpClient;
            this.roumenParser = roumenParser;
        }

        public async Task<IEnumerable<RoumenImage>> FetchImagesFromWeb()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, options.Value.DataUrl);
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("x-requested-with", "xhr");
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return roumenParser.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
    }
}