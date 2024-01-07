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

            Assert.That(result, Has.None.Matches<RoumenImage<Tag.Main>>(t => t.ImageUrl == "https://www.rouming.cz/upload/oriiisky.jpg"));
            Assert.That(result[0], Is.EqualTo(new RoumenImage<Tag.Main>("https://www.rouming.cz/upload/They-must-turn-into-salts.jpg", "They-must-turn-into-salts", "https://www.rouming.cz/roumingShow.php?file=They-must-turn-into-salts.jpg")));
            Assert.That(result[1], Is.EqualTo(new RoumenImage<Tag.Main>("https://www.rouming.cz/upload/Nobody-expects-it.jpg", "Nobody-expects-it", "https://www.rouming.cz/roumingShow.php?file=Nobody-expects-it.jpg")));
        }

        [Test]
        public void TestParserMaso()
        {
            var parser = new RoumenParser();
            var result = parser.Parse<Tag.Maso>(TestResource.MasoPage, "").ToList();

            Assert.That(result, Has.Count.EqualTo(140));
            Assert.That(result[0], Is.EqualTo(new RoumenImage<Tag.Maso>("https://www.roumenovomaso.cz/upload/Erika_je_mrdadlo_08.jpg", "Erika je mrdadlo 08", "https://www.roumenovomaso.cz/masoShow.php?file=Erika_je_mrdadlo_08.jpg")));
            Assert.That(result[1], Is.EqualTo(new RoumenImage<Tag.Maso>("https://www.roumenovomaso.cz/upload/Erika_je_mrdadlo_07.jpg", "Erika je mrdadlo 07", "https://www.roumenovomaso.cz/masoShow.php?file=Erika_je_mrdadlo_07.jpg")));
        }
    }
}
