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
                var imageNode = node.SelectSingleNode("./td[7]/a");
                var commentLink = imageNode.Attributes["href"].Value;
                var imageUrl = commentLink.Replace("/roumingShow.php?file=", "/upload/");
                var description = HtmlEntity.DeEntitize(imageNode.InnerText);

                yield return new RoumenImage(imageUrl!, description!, commentLink!);
            }
        }
    }
}