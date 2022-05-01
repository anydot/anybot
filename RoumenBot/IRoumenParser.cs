using System;
using System.Collections.Generic;

namespace RoumenBot
{
    public interface IRoumenParser
    {
        IEnumerable<RoumenImage<T>> Parse<T>(string roumingPage, Uri baseUrl) where T : Tag.TagBase, new();
    }
}