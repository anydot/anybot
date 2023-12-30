using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace RoumenBot.Tests
{
    [TestFixture]
    public class RoumenRestServiceTest
    {
        [Test]
        public async Task SanityCheckParser()
        {
            var optionsMock = new Mock<IOptions<RoumenOptions<Tag.Main>>>();
            optionsMock.SetupGet(o => o.Value).Returns(new RoumenOptions<Tag.Main>() { DataUrl = "https://www.rouming.cz/" });

            var sut = new RoumenRestService<Tag.Main>(new System.Net.Http.HttpClient(), optionsMock.Object, new RoumenParser());

            var testImages = (await sut.FetchImagesFromWeb().ConfigureAwait(false)).ToList();

            Assert.That(testImages, Has.Count.GreaterThan(50));

            var image = testImages[0];
            Assert.That(image.CommentLink, Does.StartWith("https://"));
            Assert.That(image.ImageUrl, Does.StartWith("https://"));
        }

        [Test]
        public async Task SanityCheckMasoParser()
        {
            var optionsMock = new Mock<IOptions<RoumenOptions<Tag.Maso>>>();
            optionsMock.SetupGet(o => o.Value).Returns(new RoumenOptions<Tag.Maso>() { DataUrl = "https://www.roumenovomaso.cz/?agree=on" });

            var sut = new RoumenRestService<Tag.Maso>(new System.Net.Http.HttpClient(), optionsMock.Object, new RoumenParser());

            var testImages = (await sut.FetchImagesFromWeb().ConfigureAwait(false)).ToList();

            Assert.That(testImages, Has.Count.GreaterThan(50));

            var image = testImages[0];
            Assert.That(image.CommentLink, Does.StartWith("https://"));
            Assert.That(image.ImageUrl, Does.StartWith("https://"));
        }
    }
}
