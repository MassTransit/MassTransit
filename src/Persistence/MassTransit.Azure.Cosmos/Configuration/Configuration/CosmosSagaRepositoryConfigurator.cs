namespace MassTransit.Azure.Cosmos.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Saga;
    using Metadata;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;
    using Registration;
    using Saga;
    using Saga.CollectionIdFormatters;
    using Saga.Context;

    /// <summary>
    /// A factory used to get instances of a <see cref="CosmosClient"/>
    /// </summary>
    /// <param name="cosmosClientName">The name of the client</param>
    /// <param name="serializerSettings">Serializer settings for the client</param>
    /// <returns>A <see cref="CosmosClient"/> used to interact with Cosmos DB</returns>
    public delegate CosmosClient CosmosClientFactory(string cosmosClientName, JsonSerializerSettings serializerSettings);

    public class CosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        readonly JsonSerializerSettings _serializerSettings;
        Func<IConfigurationServiceProvider, ICollectionIdFormatter> _collectionIdFormatter;
        Action<ItemRequestOptions> _itemRequestOptions;
        Action<QueryRequestOptions> _queryRequestOptions;

        public CosmosSagaRepositoryConfigurator()
        {
            _serializerSettings = GetSerializerSettingsIfNeeded();
            _collectionIdFormatter = _ => KebabCaseCollectionIdFormatter.Instance;
        }

        public void ConfigureEmulator()
        {
            EndpointUri = EmulatorConstants.EndpointUri;
            Key = EmulatorConstants.Key;
        }

        public void SetCollectionIdFormatter(ICollectionIdFormatter collectionIdFormatter)
        {
            if (collectionIdFormatter == null)
                throw new ArgumentNullException(nameof(collectionIdFormatter));
            SetCollectionIdFormatter(_ => collectionIdFormatter);
        }

        public void SetCollectionIdFormatter(Func<IConfigurationServiceProvider, ICollectionIdFormatter> collectionIdFormatterFactory)
        {
            _collectionIdFormatter = collectionIdFormatterFactory ?? throw new ArgumentNullException(nameof(collectionIdFormatterFactory));
        }

        public string DatabaseId { get; set; }

        public string CollectionId
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(value));

                _collectionIdFormatter = _ => new DefaultCollectionIdFormatter(value);
            }
        }

        public string CosmosClientName { get; set; }

        public string EndpointUri { get; set; }
        public string Key { get; set; }

        public void ConfigureItemRequestOptions(Action<ItemRequestOptions> cfg)
        {
            _itemRequestOptions = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        public void ConfigureQueryRequestOptions(Action<QueryRequestOptions> cfg)
        {
            _queryRequestOptions = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(DatabaseId))
                yield return this.Failure("DatabaseId", "must be specified");

            if (_collectionIdFormatter == null)
                yield return this.Failure("CollectionIdFormatter", "must be specified");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            DatabaseContext<TSaga> DatabaseContextFactory(IConfigurationServiceProvider provider)
            {
                var clientFactory = provider.GetService<CosmosClientFactory>();
                CosmosClient client;

                bool providerProvidedClient = true;
                if (clientFactory != null)
                {
                    client = clientFactory(CosmosClientName, _serializerSettings);
                }
                else
                {
                    providerProvidedClient = false;

                    CosmosClientOptions clientOptions = null;
                    if (_serializerSettings != null)
                    {
                        clientOptions = new CosmosClientOptions { Serializer = new CosmosJsonDotNetSerializer(_serializerSettings) };
                    }

                    client = new CosmosClient(EndpointUri, Key, clientOptions);
                }

                var collectionIdFormatter = _collectionIdFormatter(provider);
                var container = client.GetContainer(DatabaseId, collectionIdFormatter.Saga<TSaga>());

                // The provider owns the instance of the client, so the lifetime should be controlled by the provider,
                // not this class.
                return new CosmosDatabaseContext<TSaga>(providerProvidedClient ? null : client, container, _itemRequestOptions, _queryRequestOptions);
            }

            configurator.RegisterSingleInstance(DatabaseContextFactory);

            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                CosmosSagaRepositoryContextFactory<TSaga>>();
        }

        static JsonSerializerSettings GetSerializerSettingsIfNeeded()
        {
            var correlationId = TypeMetadataCache<TSaga>.Properties.Single(x => x.Name == nameof(ISaga.CorrelationId));

            if (correlationId.GetAttribute<JsonPropertyAttribute>().Any(x => x.PropertyName == "id"))
                return default;

            var resolver = new PropertyRenameSerializerContractResolver();
            resolver.RenameProperty(typeof(TSaga), nameof(ISaga.CorrelationId), "id");

            return new JsonSerializerSettings {ContractResolver = resolver};
        }
    }
}
