using NUnit.Framework;

namespace Sreality.Tests
{
    [TestFixture]
    public class EstateResponseParserTest
    {
        [Test]
        public void DeserializeTest()
        {
            var sut = new EstateResponseParser();
            var parsed = sut.Parse(TestResource.EstateResponse);

            Assert.AreEqual("/cs/v2/estates/2993966940?region_tip=531340", parsed.Embedded.Estates[0].SelfId);
        }
    }
}
