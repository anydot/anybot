using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Anybot.Common.Tests
{
    [TestFixture(StorageType.FsDb)]
    public class StorageTests
    {
        public enum StorageType
        {
            FsDb,
        }

        private readonly StorageTestHelper storage;

        public StorageTests(StorageType type)
        {
            storage = StorageTestHelper.Create(type);
        }

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

        private abstract class StorageTestHelper
        {
            private const string TestPath = "test_db";

            private class FsdbTestHelper : StorageTestHelper
            {
                public override IStorage<T> GetCollection<T>(string prefix)
                {
                    return new FsdbStorage<T>(TestPath, prefix);
                }
            }

            public static StorageTestHelper Create(StorageType type)
            {
                return type switch
                {
                    StorageType.FsDb => new FsdbTestHelper(),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid storage type"),
                };
            }

            public virtual void Close()
            {
                if (Directory.Exists(TestPath))
                {
                    Directory.Delete(TestPath, true);
                }
            }

            public virtual void Open()
            {
                if (Directory.Exists(TestPath))
                {
                    Directory.Delete(TestPath, true);
                }
            }

            public abstract IStorage<T> GetCollection<T>(string prefix);
        }

        private IStorage<TestModel> db;

        [SetUp]
        public void Setup()
        {
            storage.Open();
            db = storage.GetCollection<TestModel>("prefix");
        }

        [TearDown]
        public void Teardown()
        {
            storage.Close();
        }

        [Test]
        public void CanStoreAndReadBack()
        {
            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write("key1", data);

            Assert.That(db.TryRead("key1", out var result), Is.True);
            Assert.That(result, Is.EqualTo(data));
        }

        [Test]
        public void CanStoreKeysWithWeirdChars()
        {
            const string key = "-something://else_jej.JPG";
            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write(key, data);

            Assert.That(db.TryRead(key, out var result), Is.True);
            Assert.That(result, Is.EqualTo(data));
        }

        [Test]
        public void CanStoreAndOverwrite()
        {
            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write("key1", data);

            Assert.That(db.TryRead("key1", out var result), Is.True);
            Assert.That(result, Is.EqualTo(data));

            data.Id1 = "some-other-key";
            db.Write("key1", data);

            Assert.That(db.TryRead("key1", out result), Is.True);
            Assert.That(result, Is.EqualTo(data));
        }

        [Test]
        public void StoreDeleteAndCheck()
        {
            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write("key1", data);

            Assert.That(db.TryRead("key1", out _), Is.True);

            db.Delete("key1");

            Assert.That(db.TryRead("key1", out _), Is.False);
        }

        [Test]
        public void StoreDeleteWeirdKeyAndCheck()
        {
            const string key = "https://somekey";

            var data = new TestModel { Id1 = "id1", Id2 = "id2" };
            db.Write(key, data);

            Assert.That(db.TryRead(key, out _), Is.True);

            db.Delete(key);

            Assert.That(db.TryRead(key, out _), Is.False);
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
                Assert.That(kv.Value.Id1, Is.EqualTo(kv.Key));
                result.Add(kv.Value);
            }

            Assert.That(result, Is.EquivalentTo(data));
        }

        [Test]
        public void IterateReturnsRightKey_IfThereAreKeyWithSimilarName()
        {
            var data = Enumerable.Range(0, 10).Select(_ => new TestModel { Id1 = Guid.NewGuid().ToString(), Id2 = Guid.NewGuid().ToString() }).ToList();
            var result = new List<TestModel>();
            var result2 = new List<TestModel>();
            var db2 = storage.GetCollection<TestModel>("prefixM");

            foreach (var d in data)
            {
                db.Write(d.Id1, d);
                db2.Write(d.Id1, d);
            }

            foreach (var kv in db.Iterate())
            {
                Assert.That(kv.Value.Id1, Is.EqualTo(kv.Key));
                result.Add(kv.Value);
            }

            foreach (var kv in db2.Iterate())
            {
                Assert.That(kv.Value.Id1, Is.EqualTo(kv.Key));
                result2.Add(kv.Value);
            }

            Assert.That(result, Is.EquivalentTo(data));
            Assert.That(result2, Is.EquivalentTo(data));
        }
    }
}
