using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RoumenBot.Tag;
using System;
using System.IO;
using System.Text;

namespace RoumenBot.Tests
{
    [TestFixture]
    public class RoumenOptionsTest
    {
        [Test]
        public void TestParsingRoumingOptions()
        {
            using var configStream = new MemoryStream(Encoding.UTF8.GetBytes(TestResource.TestConfig));

            var config = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .Build();

            var collection = new ServiceCollection();
            collection.AddOptions<RoumenOptions<Main>>().Bind(config.GetSection("Roumen"));
            var provider = collection.BuildServiceProvider();
            var options = provider.GetService<IOptions<RoumenOptions<Main>>>();

            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);

            var opt = options.Value;

            Assert.AreEqual(-1001346659992L, opt.ChatId);
            Assert.AreEqual(TimeSpan.FromMinutes(47), opt.RefreshDelay);
        }
    }
}
