using Sreality.Model;

namespace Sreality
{
    public interface IEstateRecordFormatter
    {
        public string Format(EstateRecord response);
    }
}