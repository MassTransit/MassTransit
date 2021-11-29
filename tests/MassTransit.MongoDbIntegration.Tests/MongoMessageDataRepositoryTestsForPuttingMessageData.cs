namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MessageData;
    using MongoDB.Driver;
    using MongoDB.Driver.GridFS;
    using NUnit.Framework;


    [TestFixture]
    public class MongoMessageDataRepositoryTestsForPuttingMessageData
    {
        [Test]
        public async Task ThenExpirationIsNotSet()
        {
            IAsyncCursor<GridFSFileInfo> cursor = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, _filename));
            List<GridFSFileInfo> list = await cursor.ToListAsync();
            var doc = list.Single();

            Assert.That(doc.Metadata.Contains("expiration"), Is.False);
        }

        [Test]
        public async Task ThenMessageStoredAsExpected()
        {
            var result = await _bucket.DownloadAsBytesByNameAsync(_filename);

            Assert.That(result, Is.EqualTo(_expectedData));
        }

        [OneTimeSetUp]
        public async Task GivenAMongoMessageDataRepository_WhenPuttingMessageData()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);

            _expectedData = Encoding.UTF8.GetBytes("This is a test message data block");

            _resolver = new MessageDataResolver();
            _nameCreator = new StaticFileNameGenerator();
            _filename = _nameCreator.GenerateFileName();

            IMessageDataRepository repository = new MongoDbMessageDataRepository(_resolver, _bucket, _nameCreator);

            using var stream = new MemoryStream(_expectedData);

            _actualUri = await repository.Put(stream);
        }

        [OneTimeTearDown]
        public Task Kill()
        {
            return _bucket.DropAsync();
        }

        GridFSBucket _bucket;
        byte[] _expectedData;
        IFileNameGenerator _nameCreator;
        IMessageDataResolver _resolver;
        Uri _actualUri;
        string _filename;


        class StaticFileNameGenerator :
            IFileNameGenerator
        {
            public string GenerateFileName()
            {
                return "etdpgnbhbtdgjs8h1pzcbxyyyy";
            }
        }
    }
}
