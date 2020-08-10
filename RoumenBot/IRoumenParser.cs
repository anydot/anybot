using RoumenBot;
using System.Collections.Generic;

namespace RoumenBot
{
    public interface IRoumenParser
    {
        IEnumerable<RoumenImage> Parse(string roumingPage);
    }
}