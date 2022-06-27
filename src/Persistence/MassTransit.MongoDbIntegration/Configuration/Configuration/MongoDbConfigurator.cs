#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using MongoDbIntegration;
    using MongoDbIntegration.Saga;


    public abstract class MongoDbConfigurator :
        ISpecification
    {
        string? _databaseName;
        protected Func<IServiceProvider, ICollectionNameFormatter> CollectionNameFormatterFactory;
        protected Func<IServiceProvider, IMongoClient>? ProviderClientFactory;
        protected Func<IServiceProvider, IMongoDatabase>? ProviderDatabaseFactory;

        protected MongoDbConfigurator()
        {
            CollectionNameFormatterFactory = _ => DotCaseCollectionNameFormatter.Instance;
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

                ClientFactory(_ =>
                {
                    var mongoClient = new MongoClient(mongoUrl);

                    return mongoClient;
                });

                DatabaseFactory(provider =>
                {
                    var mongoClient = provider.GetRequiredService<IMongoClient>();

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

        public virtual IEnumerable<ValidationResult> Validate()
        {
            if (ProviderDatabaseFactory == null)
                yield return this.Failure("DatabaseFactory", "must be specified");

            if (string.IsNullOrWhiteSpace(_databaseName) && ProviderDatabaseFactory == null)
            {
                yield return this.Failure("DatabaseName",
                    "must be specified if no database factory is specified or if the connection string does not contain a database name");
            }
        }

        public void CollectionNameFormatter(Func<IServiceProvider, ICollectionNameFormatter> collectionNameFormatterFactory)
        {
            CollectionNameFormatterFactory = collectionNameFormatterFactory;
        }

        public void DatabaseFactory(Func<IServiceProvider, IMongoDatabase> databaseFactory)
        {
            ProviderDatabaseFactory = databaseFactory;
        }

        public void ClientFactory(Func<IServiceProvider, IMongoClient> clientFactory)
        {
            ProviderClientFactory = clientFactory;
        }
    }
}
