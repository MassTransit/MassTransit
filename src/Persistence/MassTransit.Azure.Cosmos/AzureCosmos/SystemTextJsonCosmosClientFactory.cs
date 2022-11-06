namespace MassTransit.AzureCosmos
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text.Json;
    using Microsoft.Azure.Cosmos;
    using Saga;
    using Serialization;


    /// <summary>
    /// Creates a single instance of the <see cref="CosmosClient" /> using a fixed endpoint and key
    /// for all clients, regardless of client name.
    /// </summary>
    public class SystemTextJsonCosmosClientFactory :
        ICosmosClientFactory,
        IDisposable
    {
        readonly ConcurrentDictionary<Type, Lazy<CosmosClient>> _clients;
        readonly string _endpoint;
        readonly string _key;
        readonly JsonNamingPolicy _namingPolicy;

        public SystemTextJsonCosmosClientFactory(string endpoint, string key, JsonNamingPolicy namingPolicy)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _endpoint = endpoint;
            _key = key;
            _namingPolicy = namingPolicy;

            _clients = new ConcurrentDictionary<Type, Lazy<CosmosClient>>();
        }

        public CosmosClient GetCosmosClient<T>(string clientName)
            where T : class, ISaga
        {
            return _clients.GetOrAdd(typeof(T), _ => CreateClient<T>()).Value;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<Type, Lazy<CosmosClient>> client in _clients)
            {
                if (client.Value.IsValueCreated)
                    client.Value.Value.Dispose();
            }
        }

        Lazy<CosmosClient> CreateClient<T>()
            where T : class, ISaga
        {
            return new Lazy<CosmosClient>(() =>
            {
                var clientOptions = new CosmosClientOptions();

                var options = GetSerializerOptions<T>();
                if (options != null)
                    clientOptions.Serializer = new SystemTextJsonCosmosSerializer(options);

                return new CosmosClient(_endpoint, _key, clientOptions);
            });
        }

        JsonSerializerOptions GetSerializerOptions<T>()
            where T : class, ISaga
        {
            var options = new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options)
            {
                PropertyNamingPolicy = new SagaRenamePropertyNamingPolicy(_namingPolicy)
            };

            return options;
        }
    }
}
