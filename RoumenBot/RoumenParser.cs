using HtmlAgilityPack;
using System.Collections.Generic;

namespace RoumenBot
{
    public class RoumenParser : IRoumenParser
    {
        public IEnumerable<RoumenImage<T>> Parse<T>(string roumingPage, string baseUrl) where T : Tag, new()
        {
            var doc = new HtmlDocument();

            doc.LoadHtml(roumingPage);
            var nodes = doc.DocumentNode.SelectNodes($"//div[contains(@class, '{Tag.DivName<T>()}')]/table[1]/tr");

            foreach (var node in nodes)
            {
                var imageNode = node.SelectSingleNode($"./td[{Tag.ImageTdIndex<T>()}]/a");
                var commentLink = imageNode.Attributes["href"].Value;

                if (!commentLink!.StartsWith("https://"))
                {
                    commentLink = baseUrl + "/" + commentLink;
                }

                var imageUrl = commentLink.Replace(Tag.ShowPrefix<T>(), "/upload/");
                var description = HtmlEntity.DeEntitize(imageNode.InnerText);

                yield return new RoumenImage<T>(imageUrl!, description!, commentLink!);
            }
        }
    }
}