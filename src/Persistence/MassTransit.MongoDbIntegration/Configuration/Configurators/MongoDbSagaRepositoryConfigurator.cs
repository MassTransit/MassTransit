namespace MassTransit.MongoDbIntegration.Configurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using Registration;
    using Saga;
    using Saga.CollectionNameFormatters;
    using Saga.Context;


    class MongoDbSagaRepositoryConfigurator<TSaga> :
        IMongoDbSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, IVersionedSaga
    {
        Func<IConfigurationServiceProvider, ICollectionNameFormatter> _collectionNameFormatterFactory;
        Func<IConfigurationServiceProvider, IMongoDatabase> _databaseFactory;
        Func<IConfigurationServiceProvider, BsonClassMap<TSaga>> _classMapFactory;
        string _databaseName;

        public MongoDbSagaRepositoryConfigurator()
        {
            CollectionNameFormatter(_ => DotCaseCollectionNameFormatter.Instance);
            ClassMap(provider => provider.GetService<BsonClassMap<TSaga>>() ?? new BsonClassMap<TSaga>(cfg =>
            {
                cfg.AutoMap();
                cfg.MapIdProperty(x => x.CorrelationId);
            }));
        }

        public string Connection
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value should no be empty.", nameof(Connection));

                var mongoUrl = MongoUrl.Create(value);

                if (string.IsNullOrWhiteSpace(_databaseName))
                    _databaseName = mongoUrl.DatabaseName;

                DatabaseFactory(_ =>
                {
                    var mongoClient = new MongoClient(mongoUrl);

                    return mongoClient.GetDatabase(_databaseName);
                });
            }
        }

        public string DatabaseName
        {
            set => _databaseName = value;
        }

        public string CollectionName
        {
            set { CollectionNameFormatter(_ => new DefaultCollectionNameFormatter(value)); }
        }

        public void ClassMap(Func<IConfigurationServiceProvider, BsonClassMap<TSaga>> classMapFactory)
        {
            _classMapFactory = classMapFactory;
        }

        public void CollectionNameFormatter(Func<IConfigurationServiceProvider, ICollectionNameFormatter> collectionNameFormatterFactory)
        {
            _collectionNameFormatterFactory = collectionNameFormatterFactory;
        }

        public void DatabaseFactory(Func<IConfigurationServiceProvider, IMongoDatabase> databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_databaseFactory == null)
                yield return this.Failure("DatabaseFactory", "must be specified");

            if (_collectionNameFormatterFactory == null)
                yield return this.Failure("CollectionNameFormatterFactory", "must be specified");

            if (_classMapFactory == null)
                yield return this.Failure("ClassMapFactory", "must be specified");

            if (string.IsNullOrWhiteSpace(_databaseName))
                yield return this.Failure("DatabaseName", "must be specified");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, IVersionedSaga
        {
            IMongoCollection<TSaga> MongoCollectionFactory(IConfigurationServiceProvider provider)
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof(TSaga)))
                    BsonClassMap.RegisterClassMap(_classMapFactory(provider));

                var database = _databaseFactory(provider);
                var collectionNameFormatter = _collectionNameFormatterFactory(provider);
                return database.GetCollection<TSaga>(collectionNameFormatter);
            }

            configurator.RegisterSingleInstance(MongoCollectionFactory);
            configurator.RegisterSagaRepository<T, IMongoCollection<T>, MongoDbSagaConsumeContextFactory<T>, MongoDbSagaRepositoryContextFactory<T>>();
        }
    }
}
