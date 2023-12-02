using NUnit.Framework;
using System.Linq;

namespace RoumenBot.Tests
{
    [TestFixture]
    public class RoumenParserTest
    {
        [Test]
        public void TestParserMain()
        {
            var parser = new RoumenParser();
            var result = parser.Parse<Tag.Main>(TestResource.RoumingPage, "").ToList();

            Assert.That(result, Has.Count.EqualTo(133));
            Assert.That(result[0], Is.EqualTo(new RoumenImage<Tag.Main>("https://www.rouming.cz/upload/They-must-turn-into-salts.jpg", "They-must-turn-into-salts", "https://www.rouming.cz/roumingShow.php?file=They-must-turn-into-salts.jpg")));
            Assert.That(result[1], Is.EqualTo(new RoumenImage<Tag.Main>("https://www.rouming.cz/upload/Nobody-expects-it.jpg", "Nobody-expects-it", "https://www.rouming.cz/roumingShow.php?file=Nobody-expects-it.jpg")));
        }

        [Test]
        public void TestParserMaso()
        {
            var parser = new RoumenParser();
            var result = parser.Parse<Tag.Maso>(TestResource.MasoPage, "").ToList();

            Assert.That(result, Has.Count.EqualTo(88));
            Assert.That(result[0], Is.EqualTo(new RoumenImage<Tag.Maso>("https://www.roumenovomaso.cz/upload/necum2a.jpg", "necum2a", "https://www.roumenovomaso.cz/masoShow.php?file=necum2a.jpg")));
            Assert.That(result[1], Is.EqualTo(new RoumenImage<Tag.Maso>("https://www.roumenovomaso.cz/upload/fcx3_2N4NM.jpg", "fcx3 2N4NM", "https://www.roumenovomaso.cz/masoShow.php?file=fcx3_2N4NM.jpg")));
        }
    }
}
