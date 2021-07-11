using Newtonsoft.Json;
using RocksDbSharp;
using System.Collections.Generic;

namespace Anybot.Common
{
    public class RocksStorage<T> : IStorage<T>
    {
        private readonly RocksDb db;
        private readonly string prefix;

        public RocksStorage(RocksDb db, string prefix)
        {
            this.db = db;
            this.prefix = prefix;
        }

        private string FormatKey(string key)
        {
            return string.Join(":", prefix, key);
        }

        public bool TryRead(string key, out T value)
        {
            var dbValue = db.Get(FormatKey(key));

            if (dbValue == null)
            {
                value = default!;
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(dbValue);
            return true;
        }

        public void Write(string key, T value)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            db.Put(FormatKey(key), serializedValue);
        }

        public void Delete(string key)
        {
            db.Remove(FormatKey(key));
        }

        public IEnumerable<KeyValuePair<string, T>> Iterate()
        {
            var fullPrefix = FormatKey(string.Empty);
            var fullPrefixLen = fullPrefix.Length;
            var iterator = db.NewIterator();
            iterator.Seek(fullPrefix);

            while (iterator.Valid())
            {
                var key = iterator.StringKey();

                if (!key.StartsWith(fullPrefix))
                {
                    break;
                }

                var strippedKey = key.Remove(0, fullPrefixLen);

                yield return new KeyValuePair<string, T>(strippedKey, JsonConvert.DeserializeObject<T>(iterator.StringValue()));

                iterator.Next();
            }
        }
    }
}