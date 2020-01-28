namespace MassTransit.DocumentDbIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Metadata;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using Registration;
    using Saga;
    using Saga.Context;


    public class DocumentDbSagaRepositoryConfigurator<TSaga> :
        IDocumentDbSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, IVersionedSaga
    {
        const string DefaultCollectionName = "sagas";

        readonly JsonSerializerSettings _serializerSettings;

        public DocumentDbSagaRepositoryConfigurator()
        {
            _serializerSettings = GetSerializerSettingsIfNeeded();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(DatabaseId))
                yield return this.Failure("DatabaseId", "must be specified");

            if (string.IsNullOrWhiteSpace(CollectionId))
                yield return this.Warning("CollectionId", "not specified, using default");
        }

        public void ConfigureEmulator()
        {
            EndpointUri = EmulatorConstants.EndpointUri;
            Key = EmulatorConstants.Key;
        }

        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
        public Uri EndpointUri { get; set; }
        public string Key { get; set; }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            configurator.RegisterSingleInstance(Factory);

            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, DocumentDbSagaConsumeContextFactory<TSaga>,
                DocumentDbSagaRepositoryContextFactory<TSaga>>();
        }

        DatabaseContext<TSaga> Factory(IConfigurationServiceProvider provider)
        {
            var client = new DocumentClient(EndpointUri, Key);

            var context = new DocumentDbDatabaseContext<TSaga>(client, DatabaseId, CollectionId ?? DefaultCollectionName, _serializerSettings);

            return context;
        }

        static JsonSerializerSettings GetSerializerSettingsIfNeeded()
        {
            var correlationId = TypeMetadataCache<TSaga>.Properties.Single(x => x.Name == nameof(IVersionedSaga.CorrelationId));
            var etag = TypeMetadataCache<TSaga>.Properties.Single(x => x.Name == nameof(IVersionedSaga.ETag));

            bool hasId = correlationId.GetAttribute<JsonPropertyAttribute>().Any(x => x.PropertyName == "id");
            bool hasETag = etag.GetAttribute<JsonPropertyAttribute>().Any(x => x.PropertyName == "_etag");

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
