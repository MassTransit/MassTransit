namespace MassTransit.MongoDbIntegration.Saga.Configuration
{
    using System;
    using System.Collections.Generic;
    using CollectionNameFormatters;
    using Context;
    using GreenPipes;
    using MassTransit.Saga;
    using MongoDB.Driver;
    using Registration;


    class MongoDbSagaRepositoryConfigurator<TSaga> :
        IMongoDbSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, IVersionedSaga
    {
        readonly IMongoDbSagaConsumeContextFactory _mongoDbSagaConsumeContextFactory;
        Func<IConfigurationServiceProvider, ICollectionNameFormatter> _collectionNameFormatterFactory;
        Func<IConfigurationServiceProvider, IMongoDatabase> _databaseFactory;
        string _databaseName;

        public MongoDbSagaRepositoryConfigurator()
        {
            CollectionNameFormatter(_ => DotCaseCollectionNameFormatter.Instance);
            _mongoDbSagaConsumeContextFactory = new MongoDbSagaConsumeContextFactory();
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

        public Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> BuildFactoryMethod()
        {
            ISagaRepository<TSaga> CreateRepository(IConfigurationServiceProvider provider)
            {
                var databaseFactory = _databaseFactory(provider);
                var collectionNameFormatter = _collectionNameFormatterFactory(provider);

                return new MongoDbSagaRepository<TSaga>(databaseFactory, _mongoDbSagaConsumeContextFactory, collectionNameFormatter);
            }

            return CreateRepository;
        }
    }
}
