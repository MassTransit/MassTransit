namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AzureCosmos.Saga;
    using Internals;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Newtonsoft.Json;
    using Saga;


    public class CosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISaga
    {
        readonly JsonSerializerSettings _serializerSettings;
        string _clientName;
        Func<IServiceProvider, ICosmosCollectionIdFormatter> _collectionIdFormatter;
        Action<ItemRequestOptions> _itemRequestOptions;
        Action<QueryRequestOptions> _queryRequestOptions;

        public CosmosSagaRepositoryConfigurator()
        {
            _serializerSettings = GetSerializerSettingsIfNeeded();
            _collectionIdFormatter = _ => KebabCaseCollectionIdFormatter.Instance;
        }

        public void ConfigureEmulator()
        {
            EndpointUri = AzureCosmosEmulatorConstants.EndpointUri;
            Key = AzureCosmosEmulatorConstants.Key;
        }

        public void SetCollectionIdFormatter(ICosmosCollectionIdFormatter collectionIdFormatter)
        {
            if (collectionIdFormatter == null)
                throw new ArgumentNullException(nameof(collectionIdFormatter));
            SetCollectionIdFormatter(_ => collectionIdFormatter);
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

        public void UseClientFactory(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException(nameof(clientName));

            _clientName = clientName;
        }

        public void SetCollectionIdFormatter(Func<IServiceProvider, ICosmosCollectionIdFormatter> collectionIdFormatterFactory)
        {
            _collectionIdFormatter = collectionIdFormatterFactory ?? throw new ArgumentNullException(nameof(collectionIdFormatterFactory));
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
            DatabaseContext<TSaga> DatabaseContextFactory(IServiceProvider provider)
            {
                var providerProvidedClient = true;
                CosmosClient client;

                if (!string.IsNullOrWhiteSpace(_clientName))
                {
                    var clientFactory = provider.GetRequiredService<ICosmosClientFactory>();
                    client = clientFactory.GetCosmosClient(_clientName, _serializerSettings);
                }
                else
                {
                    providerProvidedClient = false;

                    CosmosClientOptions clientOptions = null;
                    if (_serializerSettings != null)
                        clientOptions = new CosmosClientOptions { Serializer = new CosmosJsonDotNetSerializer(_serializerSettings) };

                    client = new CosmosClient(EndpointUri, Key, clientOptions);
                }

                var collectionIdFormatter = _collectionIdFormatter(provider);
                var container = client.GetContainer(DatabaseId, collectionIdFormatter.Saga<TSaga>());

                // The provider owns the instance of the client, so the lifetime should be controlled by the provider,
                // not this class.
                return new CosmosDatabaseContext<TSaga>(providerProvidedClient ? null : client, container, _itemRequestOptions, _queryRequestOptions);
            }

            configurator.TryAddSingleton(DatabaseContextFactory);
            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                CosmosSagaRepositoryContextFactory<TSaga>>();
        }

        static JsonSerializerSettings GetSerializerSettingsIfNeeded()
        {
            var correlationId = MessageTypeCache<TSaga>.Properties.Single(x => x.Name == nameof(ISaga.CorrelationId));

            if (correlationId.GetAttribute<JsonPropertyAttribute>().Any(x => x.PropertyName == "id"))
                return default;

            var resolver = new PropertyRenameSerializerContractResolver();
            resolver.RenameProperty(typeof(TSaga), nameof(ISaga.CorrelationId), "id");

            return new JsonSerializerSettings { ContractResolver = resolver };
        }
    }
}
