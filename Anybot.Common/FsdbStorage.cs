﻿using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System;

namespace Anybot.Common
{
    public class FsdbStorage<T> : IStorage<T> where T : class
    {
        private const string Temp = "tmp";
        private const string Data = "data";

        private readonly string dataRoot;
        private readonly string tmpRoot;

        public FsdbStorage(string dbRoot, string prefix)
        {
            dataRoot = Path.Combine(dbRoot, Data, prefix);
            tmpRoot = Path.Combine(dbRoot, Temp, prefix);

            Directory.CreateDirectory(dataRoot);
            Directory.CreateDirectory(tmpRoot);
        }

        public void Delete(string key)
        {
            var sanitizedKey = HttpUtility.UrlEncode(key);
            File.Delete(Path.Combine(dataRoot, sanitizedKey));
        }

        public IEnumerable<KeyValuePair<string, Func<T?>>> Iterate()
        {
            var di = new DirectoryInfo(dataRoot);

            foreach (var fi in di.GetFiles())
            {
                var key = HttpUtility.UrlDecode(fi.Name);
                yield return KeyValuePair.Create(key, () => ReadSafe(key));
            }
        }

        private T? ReadSafe(string key)
        {
            if (!TryRead(key, out var val))
            {
                return default;
            }

            return val;
        }

        public bool TryRead(string key, out T? value)
        {
            var sanitizedKey = HttpUtility.UrlEncode(key);
            var dataName = Path.Combine(dataRoot, sanitizedKey);

            try
            {
                T? retval = JsonSerializer.Deserialize<T>(File.ReadAllText(dataName, System.Text.Encoding.UTF8));

                if (retval == null)
                {
                    value = default;
                    return false;
                }

                value = retval;
                return true;
            }
            catch (FileNotFoundException)
            {
                value = default;
                return false;
            }
        }

        public void Write(string key, T value)
        {
            var sanitizedKey = HttpUtility.UrlEncode(key);
            var tmpName = Path.Combine(tmpRoot, sanitizedKey);
            var dataName = Path.Combine(dataRoot, sanitizedKey);

            using (var stream = File.Create(tmpName))
            {
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(value));
            }

            File.Move(tmpName, dataName, true);
        }
    }
}
