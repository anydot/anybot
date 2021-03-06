﻿using Microsoft.Extensions.Options;
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

            Assert.Greater(testImages.Count, 50);

            var image = testImages[0];
            StringAssert.StartsWith("https://", image.CommentLink);
            StringAssert.StartsWith("https://", image.ImageUrl);
        }
    }
}
