#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middleware;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDbIntegration;
    using MongoDbIntegration.Outbox;


    public class MongoDbOutboxConfigurator :
        MongoDbConfigurator,
        IMongoDbOutboxConfigurator
    {
        readonly IBusRegistrationConfigurator _configurator;

        public MongoDbOutboxConfigurator(IBusRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public TimeSpan DuplicateDetectionWindow { get; set; } = TimeSpan.FromMinutes(30);

        public TimeSpan QueryDelay { get; set; } = TimeSpan.FromSeconds(10);

        public int QueryMessageLimit { get; set; } = 100;

        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public void DisableInboxCleanupService()
        {
            _configurator.RemoveHostedService<InboxCleanupService>();
        }

        public virtual void UseBusOutbox(Action<IMongoDbBusOutboxConfigurator>? configure = null)
        {
            var busOutboxConfigurator = new MongoDbBusOutboxConfigurator(_configurator, this);

            busOutboxConfigurator.Configure(configure);
        }

        public virtual void Configure(Action<IMongoDbOutboxConfigurator>? configure)
        {
            configure?.Invoke(this);

            if (ProviderClientFactory == null)
                throw new ConfigurationException("ClientFactory must be specified");

            _configurator.TryAddScoped<IOutboxContextFactory<MongoDbContext>, MongoDbOutboxContextFactory>();

            _configurator.AddHostedService<InboxCleanupService>();
            _configurator.AddOptions<InboxCleanupServiceOptions>().Configure(options =>
            {
                options.DuplicateDetectionWindow = DuplicateDetectionWindow;
                options.QueryMessageLimit = QueryMessageLimit;
                options.QueryDelay = QueryDelay;
                options.QueryTimeout = QueryTimeout;
            });

            RegisterClassMaps();

            RegisterCollectionFactory<InboxState>();
            RegisterCollectionFactory<OutboxMessage>();
            RegisterCollectionFactory<OutboxState>();

            _configurator.TryAddSingleton(ProviderClientFactory);
            _configurator.TryAddScoped<MongoDbContext, TransactionMongoDbContext>();
        }

        void RegisterCollectionFactory<T>()
            where T : class
        {
            if (ProviderDatabaseFactory == null)
                throw new ConfigurationException("DatabaseFactory must be specified");

            IMongoCollection<T> CollectionFactory(IServiceProvider provider)
            {
                var database = ProviderDatabaseFactory(provider);
                var collectionNameFormatter = CollectionNameFormatterFactory(provider);

                return database.GetCollection<T>(collectionNameFormatter.Collection<T>());
            }

            _configurator.TryAddSingleton(CollectionFactory);
            _configurator.TryAddScoped(provider => provider.GetRequiredService<MongoDbContext>().GetCollection<T>());
        }

        static void RegisterClassMaps()
        {
            BsonClassMap.TryRegisterClassMap(new BsonClassMap<InboxState>(cfg =>
            {
                cfg.AutoMap();
                cfg.MapIdProperty(x => x.Id).EnsureGuidRepresentationSpecified();

                cfg.MapProperty(x => x.MessageId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.ConsumerId).EnsureGuidRepresentationSpecified();
            }));

            BsonClassMap.TryRegisterClassMap(new BsonClassMap<OutboxState>(cfg =>
            {
                cfg.AutoMap();
                cfg.MapIdProperty(x => x.OutboxId).EnsureGuidRepresentationSpecified();
            }));

            BsonClassMap.TryRegisterClassMap(new BsonClassMap<OutboxMessage>(cfg =>
            {
                cfg.AutoMap();
                cfg.MapIdProperty(x => x.Id);

                cfg.MapProperty(x => x.InboxMessageId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.InboxConsumerId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.OutboxId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.MessageId).EnsureGuidRepresentationSpecified();

                cfg.MapProperty(x => x.ConversationId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.CorrelationId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.InitiatorId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.RequestId).EnsureGuidRepresentationSpecified();
            }));
        }
    }
}
