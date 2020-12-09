using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Sreality.Model
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy))]
    public class EstateRecord
    {
        // Likely IGNORE? The labels seems to be under-used
        [JsonProperty("labelsReleased")]
        public JArray? LabelsReleased { get; init; }
        public int HasPanorama { get; init; }
        // Promo labels
        public IList<string>? Labels { get; init; }
        public bool IsAuction { get; init; }
        // Posibly useful, [0] has traits of the flat/etc
        [JsonProperty("labelsAll")]
        public IList<string>[]? LabelsAll { get; init; }
        [Obsolete]
        public JObject? Seo { get; init; }
        public bool ExclusivelyAtRk { get; init; }
        public int Category { get; init; }
        public bool HasFloorPlan { get; init; }
        // Mostly ignore, .company might be useful
        [JsonProperty("_embedded")]
        public JObject? Embedded { get; init; }
        public bool PaidLogo { get; init; }
        public string? Locality { get; init; }
        public bool HasVideo { get; init; }
        // Possibly useful links to the images (need to decode the auth etc)
        [JsonProperty("_links")]
        public JObject? Links { get; init; }
        public bool New { get; init; }
        [JsonProperty("auctionPrice")]
        public double AuctionPrice { get; init; }
        public int Type { get; init; }
        public long HashId { get; init; }
        public bool AttractiveOffer { get; init; }
        public int Price { get; init; }
        public PriceCzk? PriceCzk { get; init; }
        public bool Rus { get; init; }
        public string? Name { get; init; }
        public string? RegionTip { get; init; }
        public Gps? Gps { get; init; }
        [Obsolete]
        public bool HasMatterportUrl { get; init; }

        public string SelfId
        {
            get => Links?["self"]?["href"]?.Value<string>() ?? throw new InvalidOperationException("There's no _links or it has no .self.href string");
        }
    }
}
