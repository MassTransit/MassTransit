namespace MassTransit.AzureCosmos
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Internals;
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

        public SystemTextJsonCosmosClientFactory(string endpoint, string key)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _endpoint = endpoint;
            _key = key;

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

        static JsonSerializerOptions GetSerializerOptions<T>()
            where T : class, ISaga
        {
            var correlationId = MessageTypeCache<T>.Properties.Single(x => x.Name == nameof(ISaga.CorrelationId));

            if (correlationId.GetAttribute<JsonPropertyNameAttribute>().Any(x => x.Name == "id"))
                return SystemTextJsonMessageSerializer.Options;

            var options = new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options);
            options.PropertyNamingPolicy = new SagaRenamePropertyNamingPolicy(options.PropertyNamingPolicy);

            return options;
        }
    }
}
