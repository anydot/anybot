using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoumenBot
{
    public interface IRoumenRestService<T> where T : Tag, new()
    {
        Task<IEnumerable<RoumenImage<T>>> FetchImagesFromWeb();
    }
}