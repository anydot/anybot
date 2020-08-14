using NUnit.Framework;
using System.Linq;

namespace RoumenBot.Tests
{
    [TestFixture]
    public class RoumenParserTest
    {
        [Test]
        public void TestParser()
        {
            var parser = new RoumenParser();
            var result = parser.Parse(TestResource.RoumingPage).ToList();

            Assert.AreEqual(133, result.Count);
            Assert.AreEqual(new RoumenImage("https://www.rouming.cz/upload/They-must-turn-into-salts.jpg", "They-must-turn-into-salts", "https://www.rouming.cz/roumingShow.php?file=They-must-turn-into-salts.jpg"), result[0]);
            Assert.AreEqual(new RoumenImage("https://www.rouming.cz/upload/Nobody-expects-it.jpg", "Nobody-expects-it", "https://www.rouming.cz/roumingShow.php?file=Nobody-expects-it.jpg"), result[1]);
        }
    }
}
