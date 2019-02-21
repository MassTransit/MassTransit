namespace MassTransit.MongoDbIntegration.Saga.CollectionNameFormatters
{
    using System;
    using MassTransit.Saga;


    public class DefaultCollectionNameFormatter : ICollectionNameFormatter
    {
        readonly string _collectionName;
        const string DefaultCollectionName = "sagas";

        public DefaultCollectionNameFormatter(string collectionName = null)
        {
            collectionName = collectionName ?? DefaultCollectionName;

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (collectionName.Length > 120)
                throw new ArgumentException("Collection names must be no longer than 120 characters", nameof(collectionName));

            _collectionName = collectionName;
        }

        public string Saga<TSaga>()
            where TSaga : ISaga =>
            _collectionName;
    }
}