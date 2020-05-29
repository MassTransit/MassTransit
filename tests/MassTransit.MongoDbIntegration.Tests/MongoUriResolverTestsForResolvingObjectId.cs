namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using MessageData;
    using MongoDB.Bson;
    using NUnit.Framework;


    public class MongoUriResolverTestsForResolvingObjectId
    {
        Uri _expected;
        Uri _result;

        [SetUp]
        public void GivenAMongoMessageUriResolver_WhenResolvingAnObjectId()
        {
            var objectId = ObjectId.GenerateNewId();
            _expected = new Uri(string.Format($"urn:mongodb:gridfs:{objectId}"));
            var sut = new MessageDataResolver();

            _result = sut.GetAddress(objectId);
        }

        [Test]
        public void ThenUriFormattedCorrectly()
        {
            Assert.That(_result, Is.EqualTo(_expected));
        }
    }
}
