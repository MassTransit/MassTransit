namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using MessageData;
    using MongoDB.Bson;
    using NUnit.Framework;


    [TestFixture]
    public class MongoUriResolverTestsForResolvingUri
    {
        [Test]
        public void ThenExpectedObjectIdReturnedFromUri()
        {
            Assert.That(_result == _expected, Is.True);
        }

        [SetUp]
        public void GivenAMongoMessageUriResolver_WhenResolvingObjectIdFromUri()
        {
            _expected = ObjectId.GenerateNewId();
            var uri = new Uri(string.Format($"urn:mongodb:gridfs:{_expected}"));
            var sut = new MessageDataResolver();

            _result = sut.GetObjectId(uri);
        }

        ObjectId _expected;
        ObjectId _result;
    }
}
