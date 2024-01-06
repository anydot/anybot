namespace RoumenBot
{
#pragma warning disable S2326 // Unused type parameters should be removed
    public interface IRoumenResponseLogger<T> where T : ITag, new()
#pragma warning restore S2326 // Unused type parameters should be removed
    {
        void LogResponse(string response);
    }
}