using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ActivePass
{
    public class ActivePassRestService : IActivePassRestService
    {
        private readonly IOptions<BotOptions> options;
        private readonly HttpClient httpClient;

        public ActivePassRestService(HttpClient httpClient, IOptions<BotOptions> options)
        {
            this.options = options;
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Partner>> FetchPartnersFromWeb()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, options.Value.DataUrl);
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("x-requested-with", "xhr");
            using var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var data = await JToken.ReadFromAsync(new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false)))).ConfigureAwait(false);
            return data?["results"]?["partners"]?.Children().Select(p => p.ToObject<Partner>()) ?? Enumerable.Empty<Partner>();
        }
    }
}