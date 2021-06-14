using Anybot.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ActivePass.Tests
{
    [TestFixture]
    public class ActivePassServiceTests
    {
        private MockRepository mocks;
        private BotOptions options;
        private Mock<IOptions<BotOptions>> optionsWrapper;
        private Mock<IRocksWrapper<Partner>> dbMock;
        private Mock<ITelegramBotClient> botMock;
        private Mock<IActivePassRestService> restServiceMock;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository(MockBehavior.Strict);

            options = new BotOptions { ChatId = 42 };
            optionsWrapper = mocks.Create<IOptions<BotOptions>>(MockBehavior.Loose);

            dbMock = mocks.Create<IRocksWrapper<Partner>>(MockBehavior.Strict);
            botMock = mocks.Create<ITelegramBotClient>(MockBehavior.Strict);
            restServiceMock = mocks.Create<IActivePassRestService>(MockBehavior.Strict);

            optionsWrapper.SetupGet(o => o.Value).Returns(options);
        }

        [TearDown]
        public void Teardown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public async Task Run_FetchesPartners_GivenEmptyPartnerAndEmptyDb_DoesNothing()
        {
            restServiceMock.Setup(r => r.FetchPartnersFromWeb()).ReturnsAsync(new List<Partner>());
            dbMock.Setup(d => d.Iterate()).Returns(new List<KeyValuePair<string, Partner>>());

            var activepass = new ActivePassService(optionsWrapper.Object, Mock.Of<ILogger<ActivePassService>>(), dbMock.Object, botMock.Object, restServiceMock.Object, new NullDelayer());

            await activepass.RunOnce().ConfigureAwait(false);
            Assert.Pass();
        }

        [Test]
        public async Task Run_FetchesPartners_GivenEmptyPartnersAndOnePartnerInDB_RemovesPartnerInDb()
        {
            restServiceMock.Setup(r => r.FetchPartnersFromWeb()).ReturnsAsync(new List<Partner>());
            dbMock.Setup(d => d.Iterate()).Returns(new List<KeyValuePair<string, Partner>> { new KeyValuePair<string, Partner>("key", new Partner("", "", "key", null, "", "", "", "", "", "")) });
            dbMock.Setup(d => d.Delete("key"));
            WithSendMessage(botMock, 42, s => s.Contains("key") && s.Contains("Removed")).ReturnsAsync(Mock.Of<Telegram.Bot.Types.Message>());

            var activepass = new ActivePassService(optionsWrapper.Object, Mock.Of<ILogger<ActivePassService>>(), dbMock.Object, botMock.Object, restServiceMock.Object, new NullDelayer());

            await activepass.RunOnce().ConfigureAwait(false);
            Assert.Pass();
        }

        [Test]
        public async Task Run_FetchesPartners_GivenOldPartnerIsReturned_SendsUpdate()
        {
            var partner = new Partner("", "", "key", null, "", "", "", "", "", "");
            var partnerDb = new Partner("s", "m", "key", null, "", "", "", "", "", "");

            restServiceMock.Setup(r => r.FetchPartnersFromWeb()).ReturnsAsync(new List<Partner> { partner });
            dbMock.Setup(d => d.Iterate()).Returns(new List<KeyValuePair<string, Partner>> { new KeyValuePair<string, Partner>("key", partnerDb) });
            dbMock.Setup(d => d.Write("key", partner));
            WithSendMessage(botMock, 42, s => s.Contains("key") && s.Contains("Updated")).ReturnsAsync(Mock.Of<Telegram.Bot.Types.Message>());

            var activepass = new ActivePassService(optionsWrapper.Object, Mock.Of<ILogger<ActivePassService>>(), dbMock.Object, botMock.Object, restServiceMock.Object, new NullDelayer());

            await activepass.RunOnce().ConfigureAwait(false);
            Assert.Pass();
        }

        [Test]
        public async Task Run_FetchesPartners_GivenSamePartner_DoesNothing()
        {
            var partner = new Partner("", "", "key", null, "", "", "", "", "", "");
            var partnerDb = new Partner("", "", "key", null, "", "", "", "", "", "");

            restServiceMock.Setup(r => r.FetchPartnersFromWeb()).ReturnsAsync(new List<Partner> { partner });
            dbMock.Setup(d => d.Iterate()).Returns(new List<KeyValuePair<string, Partner>> { new KeyValuePair<string, Partner>("key", partnerDb) });

            var activepass = new ActivePassService(optionsWrapper.Object, Mock.Of<ILogger<ActivePassService>>(), dbMock.Object, botMock.Object, restServiceMock.Object, new NullDelayer());

            await activepass.RunOnce().ConfigureAwait(false);
            Assert.Pass();
        }

        public static Moq.Language.Flow.ISetup<ITelegramBotClient, Task<Telegram.Bot.Types.Message>> WithSendMessage(Mock<ITelegramBotClient> bot, long chatid, Expression<Func<string, bool>> msgMatch)
        {
            return bot.Setup(b => b.SendTextMessageAsync(It.Is<Telegram.Bot.Types.ChatId>(c => c.Identifier == chatid), It.Is(msgMatch), Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, It.IsAny<IEnumerable<Telegram.Bot.Types.MessageEntity>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()));
        }
    }
}
