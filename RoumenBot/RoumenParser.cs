using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net;

namespace RoumenBot
{
    public class RoumenParser : IRoumenParser
    {
        public IEnumerable<RoumenImage<T>> Parse<T>(string roumingPage, string baseUrl) where T : ITag, new()
        {
            var doc = new HtmlDocument();

            doc.LoadHtml(roumingPage);
            var nodes = doc.DocumentNode.SelectNodes(T.ImageLinkNodeXPath);

            foreach (var imageNode in nodes)
            {
                var commentLink = imageNode.Attributes["href"].Value;

                if (!commentLink!.StartsWith("https://"))
                {
                    commentLink = baseUrl + "/" + commentLink;
                }

                var imageUrl = commentLink.Replace(T.ShowPrefix, "/upload/");
                var description = HtmlEntity.DeEntitize(imageNode.InnerText);

                yield return new RoumenImage<T>(imageUrl!, description!, commentLink!);
            }
        }
    }
}