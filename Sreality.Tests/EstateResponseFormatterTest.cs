using System;
using NUnit.Framework;

namespace Sreality.Tests
{
    [TestFixture]
    public class EstateResponseFormatterTest
    {
        [Test]
        public void TestFormat()
        {
            var sut = new EstateRecordFormatter();
            var data = new EstateResponseParser().Parse(TestResource.EstateResponse);
            var result = sut.Format(data.Embedded.Estates[0]);

            Console.WriteLine(result);
            Assert.AreEqual("Prodej bytu 2+kk 51 m²\r\nKonselská, Praha 8 - Liben\r\n4.749M", result);
        }
    }
}
