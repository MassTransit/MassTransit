namespace MassTransit.AzureCosmos
{
    using System;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;
    using Saga;


    /// <summary>
    /// Creates a single instance of the <see cref="CosmosClient" /> using a fixed endpoint and key
    /// for all clients, regardless of client name.
    /// </summary>
    public class StaticCosmosClientFactory :
        ICosmosClientFactory,
        IDisposable
    {
        readonly string _endpoint;
        readonly object _initializationLock = new object();
        readonly string _key;

        CosmosClient _cosmosClient;

        public StaticCosmosClientFactory(string endpoint, string key)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _endpoint = endpoint;
            _key = key;
        }

        public CosmosClient GetCosmosClient(string clientName, JsonSerializerSettings serializerSettings)
        {
            if (_cosmosClient != null)
                return _cosmosClient;

            lock (_initializationLock)
            {
                _cosmosClient ??= new CosmosClient(_endpoint, _key,
                    serializerSettings == null ? null : new CosmosClientOptions { Serializer = new CosmosJsonDotNetSerializer(serializerSettings) });
            }

            return _cosmosClient;
        }

        public void Dispose()
        {
            _cosmosClient?.Dispose();
        }
    }
}
