namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDbIntegration;
    using MongoDbIntegration.Saga;
    using Saga;


    class MongoDbSagaRepositoryConfigurator<TSaga> :
        MongoDbConfigurator,
        IMongoDbSagaRepositoryConfigurator<TSaga>
        where TSaga : class, ISagaVersion
    {
        Func<IServiceProvider, BsonClassMap<TSaga>> _classMapFactory;

        public MongoDbSagaRepositoryConfigurator()
        {
            ClassMap(provider => provider.GetService<BsonClassMap<TSaga>>() ?? new BsonClassMap<TSaga>(cfg =>
            {
                cfg.AutoMap();
                cfg.MapIdProperty(x => x.CorrelationId).EnsureGuidRepresentationSpecified();
                cfg.MapProperty(x => x.Version).SetIgnoreIfDefault(false);
            }));
        }

        public void ClassMap(Func<IServiceProvider, BsonClassMap<TSaga>> classMapFactory)
        {
            _classMapFactory = classMapFactory;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_classMapFactory == null)
                yield return this.Failure("ClassMapFactory", "must be specified");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            IMongoCollection<TSaga> MongoDbCollectionFactory(IServiceProvider provider)
            {
                BsonClassMap.TryRegisterClassMap(_classMapFactory(provider));

                var database = ProviderDatabaseFactory(provider);
                var collectionNameFormatter = CollectionNameFormatterFactory(provider);

                return database.GetCollection<TSaga>(collectionNameFormatter.Saga<TSaga>());
            }

            configurator.TryAddSingleton(MongoDbCollectionFactory);

            if (ProviderClientFactory != null)
            {
                configurator.TryAddSingleton(ProviderClientFactory);
                configurator.TryAddScoped<MongoDbContext, TransactionMongoDbContext>();
            }
            else
                configurator.TryAddScoped<MongoDbContext, NoMongoDbSessionContext>();

            configurator.TryAddScoped(provider => provider.GetRequiredService<MongoDbContext>().GetCollection<TSaga>());

            configurator.RegisterLoadSagaRepository<TSaga, MongoDbSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterQuerySagaRepository<TSaga, MongoDbSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterSagaRepository<TSaga, MongoDbCollectionContext<TSaga>, SagaConsumeContextFactory<MongoDbCollectionContext<TSaga>, TSaga>,
                MongoDbSagaRepositoryContextFactory<TSaga>>();
        }
    }
}
