namespace MassTransit.AzureCosmos
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text.Json;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Options;
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
        readonly CosmosAuthSettings _authSettings;
        readonly CosmosClientOptions _clientOptions;
        readonly JsonNamingPolicy _namingPolicy;
        readonly ConcurrentDictionary<Type, Lazy<CosmosClient>> _clients;

        public SystemTextJsonCosmosClientFactory(CosmosAuthSettings authSettings, IOptions<CosmosClientOptions> clientOptions, JsonNamingPolicy namingPolicy)
        {
            if (authSettings == null)
                throw new ArgumentNullException(nameof(authSettings));

            _authSettings = authSettings;
            _clientOptions = clientOptions.Value;
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
                var options = GetSerializerOptions<T>();
                if (options != null)
                    _clientOptions.Serializer = new SystemTextJsonCosmosSerializer(options);

                return CosmosClientFactory.CreateClient(_authSettings, _clientOptions);
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
