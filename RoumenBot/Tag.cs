namespace RoumenBot
{
    public abstract class Tag
    {
        public static class TImpl<T> where T : Tag, new()
        {
            public static readonly T Instance = new T();
        }

        public static string DivName<T>() where T : Tag, new()
        {
            return TImpl<T>.Instance.DivNameInternal;
        }

        public static int ImageTdIndex<T>() where T : Tag, new()
        {
            return TImpl<T>.Instance.ImageTdIndexInternal;
        }

        public static string ShowPrefix<T>() where T : Tag, new()
        {
            return TImpl<T>.Instance.ShowPrefixInternal;
        }

        protected abstract string DivNameInternal { get; }
        protected abstract int ImageTdIndexInternal { get; }
        protected abstract string ShowPrefixInternal { get; }

        public sealed class Main : Tag
        {
            protected override string DivNameInternal => "middle";

            protected override int ImageTdIndexInternal => 7;

            protected override string ShowPrefixInternal => "/roumingShow.php?file=";
        }

        public sealed class Maso : Tag
        {
            protected override string DivNameInternal => "masoList";

            protected override int ImageTdIndexInternal => 6;

            protected override string ShowPrefixInternal => "/masoShow.php?file=";
        }
    }
}