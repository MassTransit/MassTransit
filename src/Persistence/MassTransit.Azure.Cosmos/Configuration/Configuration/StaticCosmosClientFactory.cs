namespace MassTransit.Azure.Cosmos.Configuration
{
    using System;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    /// <summary>
    /// Creates a single instance of the <see cref="CosmosClient"/> using a fixed endpoint and key
    /// for all clients, regardless of client name.
    /// </summary>
    public class StaticCosmosClientFactory : ICosmosClientFactory, IDisposable
    {
        readonly string _endpoint;
        readonly string _key;
        readonly CosmosClientOptions _cosmosClientOptions;

        CosmosClient _cosmosClient;
        readonly object _initializationLock = new object();

        public StaticCosmosClientFactory(string endpoint, string key) :
            this(endpoint, key, new CosmosClientOptions())
        {
        }

        public StaticCosmosClientFactory(string endpoint, string key, CosmosClientOptions cosmosClientOptions)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _endpoint = endpoint;
            _key = key;
            _cosmosClientOptions = cosmosClientOptions ?? throw new ArgumentNullException(nameof(cosmosClientOptions));
        }

        public CosmosClient GetCosmosClient(string cosmosClientName, JsonSerializerSettings serializerSettings)
        {
            if (_cosmosClient == null)
            {
                lock (_initializationLock)
                {
                    if (_cosmosClient == null)
                    {
                        if (serializerSettings != null)
                        {
                            _cosmosClientOptions.Serializer = new CosmosJsonDotNetSerializer(serializerSettings);
                        }

                        _cosmosClient = new CosmosClient(
                            _endpoint,
                            _key,
                            _cosmosClientOptions);
                    }
                }
            }

            return _cosmosClient;
        }

        public void Dispose()
        {
            _cosmosClient?.Dispose();
        }
    }
}
