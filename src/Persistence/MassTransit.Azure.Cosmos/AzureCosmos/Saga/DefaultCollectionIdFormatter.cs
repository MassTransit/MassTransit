namespace MassTransit.AzureCosmos.Saga
{
    using System;


    public class DefaultCollectionIdFormatter :
        ICosmosCollectionIdFormatter
    {
        const string DefaultCollectionName = "sagas";
        readonly string _collectionName;

        public DefaultCollectionIdFormatter(string collectionName = null)
        {
            collectionName ??= DefaultCollectionName;

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (collectionName.Length > 120)
                throw new ArgumentException("Collection id must be no longer than 120 characters", nameof(collectionName));

            _collectionName = collectionName;
        }

        public string Saga<TSaga>()
            where TSaga : ISaga
        {
            return _collectionName;
        }
    }
}
