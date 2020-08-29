using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ActivePass.Tests
{
    [TestFixture]
    [Explicit]
    public class ActivePassRestServiceTests
    {
        [Test]
        public async Task RestServiceRetrievesSomeData()
        {
            var options = new BotOptions { DataUrl = "https://www.activepass.cz/aktivity?activities=%5B%5D&locations=%5B%22Praha%22%5D&gps_latitude=-1&gps_longitude=-1&orderBy=asc&page=1&resultsPerPage=1800&searchString=&partnerSearch=true&activitySearch=false&view=list&sortBy=alphabet&max_north=0&max_east=0&min_north=0&min_east=0&max_north=undefined&max_east=undefined&min_north=undefined&min_east=undefined" };
            var optionsMock = new Mock<IOptions<BotOptions>>();
            optionsMock.SetupGet(o => o.Value).Returns(options);

            var sut = new ActivePassRestService(new System.Net.Http.HttpClient(), optionsMock.Object);
            var result = (await sut.FetchPartnersFromWeb().ConfigureAwait(false)).ToList();

            Assert.IsTrue(result.Count > 0);
            Assert.IsNotNull(result[0].PartnerId);
            Assert.IsNotEmpty(result[0].PartnerId);
        }
    }
}
