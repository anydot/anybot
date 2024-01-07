using System;
using System.Collections.Generic;

namespace Anybot.Common
{
    public interface IStorage<T>
    {
        void Delete(string key);
        IEnumerable<KeyValuePair<string, Func<T?>>> Iterate();
        bool TryRead(string key, out T? value);
        void Write(string key, T value);
    }
}