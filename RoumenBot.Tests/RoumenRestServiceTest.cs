using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RoumenBot.Tag;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoumenBot.Tests
{
    [TestFixture]
    public class RoumenRestServiceTest
    {
        [Test]
        public async Task SanityCheckParser()
        {
            var optionsMock = new Mock<IOptions<RoumenOptions<Main>>>();
            optionsMock.SetupGet(o => o.Value).Returns(new RoumenOptions<Main>() { DataUrl = "https://www.rouming.cz/" });

            using var httpClient = new HttpClient();

            var sut = new RoumenRestService<Main>(httpClient, optionsMock.Object, new RoumenParser());

            var testImages = (await sut.FetchImagesFromWeb().ConfigureAwait(false)).ToList();

            Assert.Greater(testImages.Count, 50);

            var image = testImages[0];
            StringAssert.StartsWith("https://", image.CommentLink);
            StringAssert.StartsWith("https://", image.ImageUrl);
        }
    }
}
