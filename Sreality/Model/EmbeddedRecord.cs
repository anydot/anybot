using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Sreality.Model
{
    [JsonObject(NamingStrategyType = typeof(Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy))]
    public class EmbeddedRecord
    {
        public IList<EstateRecord>? Estates { get; init; }
        [Obsolete]
        public JObject? IsSaved { get; init; }
        [Obsolete]
        public JObject? NotPreciseLocationCount { get; init; }
    }
}
