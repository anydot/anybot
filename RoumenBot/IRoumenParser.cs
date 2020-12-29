using System.Collections.Generic;

namespace RoumenBot
{
    public interface IRoumenParser
    {
        IEnumerable<RoumenImage<T>> Parse<T>(string roumingPage, string baseUrl) where T : Tag, new();
    }
}