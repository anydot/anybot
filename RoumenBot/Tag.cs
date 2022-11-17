namespace RoumenBot
{
    public interface ITag
    {
        static abstract string DivName { get; }

        static abstract int ImageTdIndex { get; }
        static abstract string ShowPrefix { get; }
    }
}