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
        IMongoDbSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISagaVersion
    {
        Func<IServiceProvider, BsonClassMap<TSaga>> _classMapFactory;
        Func<IServiceProvider, ICollectionNameFormatter> _collectionNameFormatterFactory;
        Func<IServiceProvider, IMongoDatabase> _databaseFactory;
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

        public void ClassMap(Func<IServiceProvider, BsonClassMap<TSaga>> classMapFactory)
        {
            _classMapFactory = classMapFactory;
        }

        public void CollectionNameFormatter(Func<IServiceProvider, ICollectionNameFormatter> collectionNameFormatterFactory)
        {
            _collectionNameFormatterFactory = collectionNameFormatterFactory;
        }

        public void DatabaseFactory(Func<IServiceProvider, IMongoDatabase> databaseFactory)
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

            if (string.IsNullOrWhiteSpace(_databaseName) && _databaseFactory == null)
                yield return this.Failure("DatabaseName",
                    "must be specified if no database factory is specified or if the connection string does not contain a database name");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, ISagaVersion
        {
            IMongoCollection<TSaga> MongoCollectionFactory(IServiceProvider provider)
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof(TSaga)))
                    BsonClassMap.RegisterClassMap(_classMapFactory(provider));

                var database = _databaseFactory(provider);
                var collectionNameFormatter = _collectionNameFormatterFactory(provider);
                return database.GetCollection<TSaga>(collectionNameFormatter);
            }

            configurator.TryAddSingleton(MongoCollectionFactory);
            configurator.RegisterSagaRepository<T, IMongoCollection<T>, SagaConsumeContextFactory<IMongoCollection<T>, T>,
                MongoDbSagaRepositoryContextFactory<T>>();
        }
    }
}
