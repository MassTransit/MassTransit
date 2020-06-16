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


    public class CosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, IVersionedSaga
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
                var client = new CosmosClient(EndpointUri, Key, new CosmosClientOptions {Serializer = new CosmosJsonDotNetSerializer(_serializerSettings)});
                var collectionIdFormatter = _collectionIdFormatter(provider);
                var container = client.GetContainer(DatabaseId, collectionIdFormatter.Saga<TSaga>());

                return new CosmosDatabaseContext<TSaga>(client, container, _itemRequestOptions, _queryRequestOptions);
            }

            configurator.RegisterSingleInstance(DatabaseContextFactory);

            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                CosmosSagaRepositoryContextFactory<TSaga>>();
        }

        static JsonSerializerSettings GetSerializerSettingsIfNeeded()
        {
            var correlationId = TypeMetadataCache<TSaga>.Properties.Single(x => x.Name == nameof(IVersionedSaga.CorrelationId));
            var etag = TypeMetadataCache<TSaga>.Properties.Single(x => x.Name == nameof(IVersionedSaga.ETag));

            var hasId = correlationId.GetAttribute<JsonPropertyAttribute>().Any(x => x.PropertyName == "id");
            var hasETag = etag.GetAttribute<JsonPropertyAttribute>().Any(x => x.PropertyName == "_etag");

            if (hasId && hasETag)
                return default;

            var resolver = new PropertyRenameSerializerContractResolver();
            if (!hasId)
                resolver.RenameProperty(typeof(TSaga), nameof(IVersionedSaga.CorrelationId), "id");
            if (!hasETag)
                resolver.RenameProperty(typeof(TSaga), nameof(IVersionedSaga.ETag), "_etag");

            return new JsonSerializerSettings {ContractResolver = resolver};
        }
    }
}
