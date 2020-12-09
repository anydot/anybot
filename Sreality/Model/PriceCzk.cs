using Newtonsoft.Json;

namespace Sreality.Model
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy))]
    public class PriceCzk
    {
        public int ValueRaw { get; init; }
        public string? Unit { get; init; }
        public string? Name { get; init; }
    }
}
