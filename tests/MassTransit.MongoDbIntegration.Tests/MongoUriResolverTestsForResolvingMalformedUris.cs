namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using MessageData;
    using NUnit.Framework;


    [TestFixture("abc:mongodb:gridfs:12345")]
    [TestFixture("urn:mongodb:gridfs:somethingelse:12345")]
    [TestFixture("urn:mongodb:gridfsthing:12345")]
    public class MongoUriResolverTestsForResolvingMalformedUris
    {
        [Test]
        public void ThenExceptionIsUriFormatException()
        {
            Assert.That(_exception, Is.TypeOf<UriFormatException>());
        }

        [SetUp]
        public void GivenAMongoMessageUriResolver_WhenResolvingMalformedUris()
        {
            var sut = new MessageDataResolver();

            try
            {
                sut.GetObjectId(_uri);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        Exception _exception;
        readonly Uri _uri;

        public MongoUriResolverTestsForResolvingMalformedUris(string uri)
        {
            _uri = new Uri(uri);
        }
    }
}
