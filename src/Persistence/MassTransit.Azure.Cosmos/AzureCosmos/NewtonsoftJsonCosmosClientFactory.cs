namespace MassTransit.AzureCosmos
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Internals;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;
    using Saga;


    /// <summary>
    /// Creates a single instance of the <see cref="CosmosClient" /> using a fixed endpoint and key
    /// for all clients, regardless of client name.
    /// </summary>
    public class NewtonsoftJsonCosmosClientFactory :
        ICosmosClientFactory,
        IDisposable
    {
        readonly ConcurrentDictionary<Type, Lazy<CosmosClient>> _clients;
        readonly CosmosAuthSettings _authSettings;

        public NewtonsoftJsonCosmosClientFactory(CosmosAuthSettings authSettings)
        {
            if (authSettings == null)
                throw new ArgumentNullException(nameof(authSettings));

            _authSettings = authSettings;

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

                var serializerSettings = GetSerializerSettingsIfNeeded<T>();
                if (serializerSettings != null)
                    clientOptions.Serializer = new NewtonsoftJsonCosmosSerializer(serializerSettings);

                return CosmosClientFactory.CreateClient(_authSettings, clientOptions);
            });
        }

        static JsonSerializerSettings GetSerializerSettingsIfNeeded<T>()
            where T : class, ISaga
        {
            var correlationId = MessageTypeCache<T>.Properties.Single(x => x.Name == nameof(ISaga.CorrelationId));

            if (correlationId.GetAttribute<JsonPropertyAttribute>().Any(x => x.PropertyName == "id"))
                return null;

            var resolver = new PropertyRenameSerializerContractResolver();
            resolver.RenameProperty(typeof(T), nameof(ISaga.CorrelationId), "id");

            return new JsonSerializerSettings { ContractResolver = resolver };
        }
    }
}
