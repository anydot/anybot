using System.Collections.Generic;

namespace RoumenBot
{
    public interface IRoumenParser
    {
        IEnumerable<RoumenImage<T>> Parse<T>(string roumingPage) where T : Tag, new();
    }
}