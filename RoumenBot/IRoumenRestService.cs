using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoumenBot
{
    public interface IRoumenRestService
    {
        Task<IEnumerable<RoumenImage>> FetchImagesFromWeb();
    }
}