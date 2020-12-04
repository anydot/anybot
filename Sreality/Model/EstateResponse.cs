using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sreality.Model
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy))]
    public class EstateResponse
    {
        public string? MetaDescription { get; init; }
        public int ResultSize { get; init; }
        [JsonProperty("_embedded")]
        public EmbeddedRecord? Embedded { get; init; }
        [JsonProperty("filterLabels")]
        public IList<string>? FilterLabels { get; init; }
        public string? Title { get; init; }
        public IReadOnlyDictionary<string, string>? Filter { get; init; }
        [JsonProperty("_links")]
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? Links { get; init; }
        public string? Locality { get; init; }
        public string? LocalityDativ { get; init; }
        public bool LoggedIn { get; init; }
        public int PerPage { get; init; }
        public string? CategoryInstrumental { get; init; }
        public int? Page { get; init; }
        [JsonProperty("filterLabels2")]
        public IReadOnlyDictionary<string, string>? FilterLabels2 { get; init; }
    }
}
