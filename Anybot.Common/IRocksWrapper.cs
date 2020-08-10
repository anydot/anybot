using System.Collections.Generic;

namespace Anybot.Common
{
    public interface IRocksWrapper<T>
    {
        void Delete(string key);
        IEnumerable<KeyValuePair<string, T>> Iterate();
        bool TryRead(string key, out T value);
        void Write(string key, T value);
    }
}