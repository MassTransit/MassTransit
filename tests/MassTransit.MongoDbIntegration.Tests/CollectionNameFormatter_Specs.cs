namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using MongoDbIntegration.Saga;
    using NUnit.Framework;
    using Saga.Data;


    [TestFixture]
    public class DotCollectionNameFormatter_Specs
    {
        [Test]
        public void Should_return_correct_collection()
        {
            var collectionName = _collectionNameFormatter.Saga<SimpleSaga>();
            Assert.AreEqual("simple.sagas", collectionName);
        }

        readonly ICollectionNameFormatter _collectionNameFormatter;

        public DotCollectionNameFormatter_Specs()
        {
            _collectionNameFormatter = new DotCaseCollectionNameFormatter();
        }
    }


    [TestFixture]
    public class DefaultCollectionNameFormatter_Specs
    {
        [Test]
        public void Should_return_default_collection_when_null()
        {
            var collectionName = _collectionNameFormatter(null).Saga<SimpleSaga>();
            Assert.AreEqual("sagas", collectionName);
        }

        readonly Func<string, ICollectionNameFormatter> _collectionNameFormatter;

        public DefaultCollectionNameFormatter_Specs()
        {
            _collectionNameFormatter = str => new DefaultCollectionNameFormatter(str);
        }

        [Theory]
        [TestCase("sagas", "sagas")]
        [TestCase("simple.sagas", "simple.sagas")]
        public void Should_return_correct_collection(string expected, string result)
        {
            var collectionName = _collectionNameFormatter(expected).Saga<SimpleSaga>();
            Assert.AreEqual(result, collectionName);
        }
    }
}
