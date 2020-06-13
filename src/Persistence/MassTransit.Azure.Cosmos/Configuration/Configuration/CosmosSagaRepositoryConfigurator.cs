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
    using Saga.Context;


    public class CosmosSagaRepositoryConfigurator<TSaga> :
        ICosmosSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, IVersionedSaga
    {
        const string DefaultCollectionName = "sagas";

        readonly JsonSerializerSettings _serializerSettings;

        public CosmosSagaRepositoryConfigurator()
        {
            _serializerSettings = GetSerializerSettingsIfNeeded();
        }

        public void ConfigureEmulator()
        {
            EndpointUri = EmulatorConstants.EndpointUri;
            Key = EmulatorConstants.Key;
        }

        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
        public string EndpointUri { get; set; }
        public string Key { get; set; }
        public Action<ItemRequestOptions> ItemRequestOptions { get; set; }
        public Action<QueryRequestOptions> QueryRequestOptions { get; set; }
        public Func<TSaga, PartitionKey> PartitionKeyExpression { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(DatabaseId))
                yield return this.Failure("DatabaseId", "must be specified");

            if (string.IsNullOrWhiteSpace(CollectionId))
                yield return this.Warning("CollectionId", "not specified, using default");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            configurator.RegisterSingleInstance(Factory);

            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                CosmosSagaRepositoryContextFactory<TSaga>>();
        }

        DatabaseContext<TSaga> Factory(IConfigurationServiceProvider provider)
        {
            var client = new CosmosClient(EndpointUri, Key, new CosmosClientOptions { Serializer = new CosmosJsonDotNetSerializer(_serializerSettings) });

            var context = new CosmosDatabaseContext<TSaga>(client, DatabaseId, CollectionId ?? DefaultCollectionName, ItemRequestOptions, QueryRequestOptions, PartitionKeyExpression);

            return context;
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

        public void ConfigureItemRequestOptions(Action<ItemRequestOptions> cfg)
        {
            ItemRequestOptions = cfg;
        }

        public void ConfigureQueryRequestOptions(Action<QueryRequestOptions> cfg)
        {
            QueryRequestOptions = cfg;
        }

        public void AddPartitionKeyExpression(Func<TSaga, PartitionKey> expression)
        {
            PartitionKeyExpression = expression;
        }
    }
}
