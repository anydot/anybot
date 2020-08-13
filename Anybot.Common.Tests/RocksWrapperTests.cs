using NUnit.Framework;
using RocksDbSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Anybot.Common.Tests
{
    [TestFixture]
    public class RocksWrapperTests
    {
        public class TestModel
        {
            public string Id1 { get; set; }
            public string Id2 { get; set; }

            public override bool Equals(object obj)
            {
                return obj is TestModel model &&
                       Id1 == model.Id1 &&
                       Id2 == model.Id2;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id1, Id2);
            }
        }

        private RocksDb rocksDb;
        private RocksWrapper<TestModel> db;

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists("testdb"))
            {
                Directory.Delete("testdb", true);
            }

            rocksDb = RocksDb.Open(new DbOptions().SetCreateIfMissing(true), "testdb");
            db = new RocksWrapper<TestModel>(rocksDb, "prefix");
        }

        [TearDown]
        public void Teardown()
        {
            rocksDb.Dispose();
            rocksDb = null;
            db = null;

            Directory.Delete("testdb", true);
        }

        [Test]
        public void CanStoreAndReadBack()
        {
            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write("key1", data);

            Assert.IsTrue(db.TryRead("key1", out var result));
            Assert.AreEqual(data, result);
        }

        [Test]
        public void CanStoreAndOverwrite()
        {
            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write("key1", data);

            Assert.IsTrue(db.TryRead("key1", out var result));
            Assert.AreEqual(data, result);

            data.Id1 = "someotherkey";
            db.Write("key1", data);

            Assert.IsTrue(db.TryRead("key1", out result));
            Assert.AreEqual(data, result);
        }

        [Test]
        public void StoreDeleteAndCheck()
        {
            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write("key1", data);

            Assert.IsTrue(db.TryRead("key1", out _));

            db.Delete("key1");

            Assert.IsFalse(db.TryRead("key1", out _));
        }

        [Test]
        public void CanIterate()
        {
            var data = Enumerable.Range(0, 10).Select(_ => new TestModel { Id1 = Guid.NewGuid().ToString(), Id2 = Guid.NewGuid().ToString() }).ToList();
            var result = new List<TestModel>();

            foreach (var d in data)
            {
                db.Write(d.Id1, d);
            }

            foreach (var kv in db.Iterate())
            {
                Assert.AreEqual(kv.Key, kv.Value.Id1);
                result.Add(kv.Value);
            }

            CollectionAssert.AreEquivalent(data, result);
        }
    }
}
