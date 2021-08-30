namespace Anybot.Common
{
    public class FsdbProvider
    {
        private readonly string path;

        public FsdbProvider(string path)
        {
            this.path = path;
        }

        public IStorage<T> Create<T>(string prefix)
        {
            return new FsdbStorage<T>(path, prefix);
        }
    }
}