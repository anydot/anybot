using Sreality.Model;

namespace Sreality
{
    public interface IEstateResponseParser
    {
        public EstateResponse Parse(string data);
    }
}