using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActivePass
{
    public interface IActivePassRestService
    {
        Task<IEnumerable<Partner>> FetchPartnersFromWeb();
    }
}