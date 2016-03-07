using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiberisLabs.MassTransit.MessageData.MongoDb.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoMessageDataRepositoryTests
{
    [TestFixture]
    public class MongoMessageDataRepositoryTestsForPuttingMessageDataWithExpiration
    {
        private GridFSBucket _bucket;
        private TimeSpan _expectedTtl;
        private DateTime _now;
        private ObjectId _id;

        [OneTimeSetUp]
        public void GivenAMongoMessageDataRepository_WhenPuttingMessageDataWithExpiration()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);

            _now = DateTime.UtcNow;
            SystemDateTime.Set(_now);

            var fixture = new Fixture();

            var resolver = new Mock<IMongoMessageUriResolver>();
            resolver.Setup(x => x.Resolve(It.IsAny<ObjectId>()))
                .Callback((ObjectId id) => _id = id);

            var nameCreator = new Mock<IFileNameCreator>();
            nameCreator.Setup(x => x.CreateFileName())
                .Returns(fixture.Create<string>());
            
            var sut = new MongoMessageDataRepository(resolver.Object, _bucket, nameCreator.Object);
            _expectedTtl = TimeSpan.FromHours(1);

            using (var stream = new MemoryStream(fixture.Create<byte[]>()))
            {
                sut.Put(stream, _expectedTtl).GetAwaiter().GetResult();
            }
        }
     
        [Test]
        public async Task ThenExpirationSetAsExpected()
        {
            var cursor = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", _id));
            var list = await cursor.ToListAsync();
            var doc = list.Single();

            var expiration = doc.Metadata["expiration"].ToUniversalTime();
            Assert.That(expiration, Is.EqualTo(_now.Add(_expectedTtl)).Within(1).Milliseconds);
        }

        [OneTimeTearDown]
        public void Kill()
        {
            SystemDateTime.Reset();

            _bucket.DropAsync().GetAwaiter().GetResult();
        }
    }
}