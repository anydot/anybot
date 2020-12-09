using Sreality.Model;
using Newtonsoft.Json;

namespace Sreality
{
    public class EstateResponseParser : IEstateResponseParser
    {
        public EstateResponse Parse(string data)
        {
            var settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
            return JsonConvert.DeserializeObject<EstateResponse>(data, settings)!;
        }
    }
}
