using System.Collections.Generic;
using HtmlAgilityPack;

namespace RoumenBot
{
    public class RoumenParser : IRoumenParser
    {
        public IEnumerable<RoumenImage> Parse(string roumingPage)
        {
            var doc = new HtmlDocument();

            doc.LoadHtml(roumingPage);
            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'middle')]/table[1]/tr");

            foreach (var node in nodes)
            {
                var commentLink = "https://www.rouming.cz/" + node.SelectSingleNode("./td[3]/a").Attributes["href"].Value;
                var imageNode = node.SelectSingleNode("./td[7]/a");
                var imageUrl = imageNode.Attributes["href"].Value.Replace("/roumingShow.php?file=", "/upload/");
                var description = HtmlEntity.DeEntitize(imageNode.InnerText);

                yield return new RoumenImage(imageUrl!, description!, commentLink!);
            }
        }
    }
}