using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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
            collection.AddOptions<RoumenOptions<Tag.Main>>().Bind(config.GetSection("Roumen"));
            var provider = collection.BuildServiceProvider();
            var options = provider.GetService<IOptions<RoumenOptions<Tag.Main>>>();

            Assert.That(options, Is.Not.Null);
            Assert.That(options.Value, Is.Not.Null);

            var opt = options.Value;

            Assert.That(opt.ChatId, Is.EqualTo(-1001346659992L));
            Assert.That(opt.RefreshDelay, Is.EqualTo(TimeSpan.FromMinutes(47)));
        }
    }
}
