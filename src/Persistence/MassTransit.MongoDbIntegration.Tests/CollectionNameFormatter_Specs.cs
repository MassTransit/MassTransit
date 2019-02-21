namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using MongoDbIntegration.Saga.CollectionNameFormatters;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public class DotCollectionNameFormatter_Specs
    {
        readonly ICollectionNameFormatter _collectionNameFormatter;

        public DotCollectionNameFormatter_Specs()
        {
            _collectionNameFormatter = new DotCaseCollectionNameFormatter();
        }

        [Test]
        public void Should_return_correct_collection()
        {
            var collectionName = _collectionNameFormatter.Saga<SimpleSaga>();
            Assert.AreEqual("simple.sagas", collectionName);
        }
    }


    [TestFixture]
    public class DefaultCollectionNameFormatter_Specs
    {
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

        [Test]
        public void Should_return_default_collection_when_null()
        {
            var collectionName = _collectionNameFormatter(null).Saga<SimpleSaga>();
            Assert.AreEqual("sagas", collectionName);
        }
    }
}
