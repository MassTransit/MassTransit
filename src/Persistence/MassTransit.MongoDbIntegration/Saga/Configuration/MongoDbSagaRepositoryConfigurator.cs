namespace MassTransit.MongoDbIntegration.Saga.Configuration
{
    using System;
    using System.Collections.Generic;
    using CollectionNameFormatters;
    using Context;
    using GreenPipes;
    using MongoDB.Driver;
    using Registration;


    class MongoDbSagaRepositoryConfigurator<TSaga> :
        IMongoDbSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, IVersionedSaga
    {
        Func<IConfigurationServiceProvider, ICollectionNameFormatter> _collectionNameFormatterFactory;
        Func<IConfigurationServiceProvider, IMongoDatabase> _databaseFactory;
        string _databaseName;

        public MongoDbSagaRepositoryConfigurator()
        {
            CollectionNameFormatter(_ => DotCaseCollectionNameFormatter.Instance);
        }

        public string Connection
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value should no be empty.", nameof(Connection));
                var mongoUrl = MongoUrl.Create(value);
                var mongoClient = new MongoClient(mongoUrl);
                if (string.IsNullOrWhiteSpace(_databaseName))
                    _databaseName = mongoUrl.DatabaseName;
                DatabaseFactory(_ => mongoClient.GetDatabase(_databaseName));
            }
        }

        public string DatabaseName
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value should no be empty.", nameof(DatabaseName));
                _databaseName = value;
            }
        }

        public string CollectionName
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Value should no be empty.", nameof(CollectionName));
                CollectionNameFormatter(_ => new DefaultCollectionNameFormatter(value));
            }
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

            if (string.IsNullOrWhiteSpace(_databaseName))
                yield return this.Failure("DatabaseName", "must be specified");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, IVersionedSaga
        {
            IMongoCollection<TSaga> MongoCollectionFactory(IConfigurationServiceProvider provider)
            {
                var database = _databaseFactory(provider);
                var collectionNameFormatter = _collectionNameFormatterFactory(provider);
                return database.GetCollection<TSaga>(collectionNameFormatter);
            }

            configurator.Register(MongoCollectionFactory);
            configurator.RegisterSagaRepository<T, IMongoCollection<T>, MongoDbSagaConsumeContextFactory<T>, MongoDbSagaRepositoryContextFactory<T>>();
        }
    }
}
