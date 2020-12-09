using System.Text;
using Sreality.Model;

namespace Sreality
{
    public class EstateRecordFormatter : IEstateRecordFormatter
    {
        public string Format(EstateRecord record)
        {
            var sb = new StringBuilder();

            sb.AppendLine(record.Name);
            sb.AppendLine(record.Locality);
            sb.AppendLine($"{record.Price / 1000_000.0:0.000}M CZK");
            sb.AppendLine($"Link: https://www.sreality.cz/detail/prodej/a/b/c/{record.HashId}");
            sb.AppendLine($"Image: {record.Links?["images"]?[0]?["href"]}");

            return sb.ToString();
        }
    }
}