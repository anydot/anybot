using System.Text.Json;
using System.Collections.Generic;
using System.IO;

namespace Anybot.Common
{
    public class FsdbStorage<T> : IStorage<T>
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
            File.Delete(Path.Combine(dataRoot, key));
        }

        public IEnumerable<KeyValuePair<string, T>> Iterate()
        {
            var di = new DirectoryInfo(dataRoot);

            foreach (var fi in di.GetFiles())
            {
                if (TryRead(fi.Name, out var val))
                {
                    yield return KeyValuePair.Create(fi.Name, val);
                }
            }
        }

        public bool TryRead(string key, out T value)
        {
            var dataName = Path.Combine(dataRoot, key);

            try
            {
                value = JsonSerializer.Deserialize<T>(File.ReadAllText(dataName, System.Text.Encoding.UTF8));
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
            var tmpName = Path.Combine(tmpRoot, key);
            var dataName = Path.Combine(dataRoot, key);

            using (var stream = File.Create(tmpName))
            {
                stream.Write(JsonSerializer.SerializeToUtf8Bytes(value));
            }

            File.Move(tmpName, dataName, true);
        }
    }
}