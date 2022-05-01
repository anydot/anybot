namespace RoumenBot.Tag
{
    public abstract class TagBase
    {
        private static class TImpl<T> where T : TagBase, new()
        {
            public static readonly T Instance = new();
        }

        public static string DivName<T>() where T : TagBase, new()
        {
            return TImpl<T>.Instance.DivNameInternal;
        }

        public static int ImageTdIndex<T>() where T : TagBase, new()
        {
            return TImpl<T>.Instance.ImageTdIndexInternal;
        }

        public static string ShowPrefix<T>() where T : TagBase, new()
        {
            return TImpl<T>.Instance.ShowPrefixInternal;
        }

        protected abstract string DivNameInternal { get; }
        protected abstract int ImageTdIndexInternal { get; }
        protected abstract string ShowPrefixInternal { get; }
    }

}