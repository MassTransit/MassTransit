namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Amazon.DynamoDBv2.DataModel;
    using DynamoDbIntegration.Saga;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;


    public class DynamoDbSagaRepositoryConfigurator<TSaga> :
        IDynamoDbSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISagaVersion
    {
        Func<IServiceProvider, IDynamoDBContext> _contextFactory;

        public DynamoDbSagaRepositoryConfigurator()
        {
            LockTimeout = TimeSpan.FromMinutes(30);
        }

        public string TableName { get; set; }
        public string LockSuffix { get; set; }
        public TimeSpan LockTimeout { get; set; }
        public TimeSpan? Expiration { get; set; }

        public void ContextFactory(Func<IDynamoDBContext> contextFactory)
        {
            _contextFactory = provider => contextFactory();
        }

        public void ContextFactory(Func<IServiceProvider, IDynamoDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_contextFactory == null)
                yield return this.Failure("ContextFactory", "must be specified");
            if (string.IsNullOrWhiteSpace(TableName))
                yield return this.Failure("TableName", "must be specified");
            if (LockTimeout <= TimeSpan.Zero)
                yield return this.Failure("LockTimeout", "Must be > TimeSpan.Zero");
            if (Expiration < TimeSpan.FromSeconds(30))
                yield return this.Failure("Expiration", "If specified, must be > 30 seconds");
        }

        public void Register(ISagaRepositoryRegistrationConfigurator<TSaga> configurator)
        {
            configurator.TryAddSingleton(_contextFactory);
            configurator.TryAddSingleton(new DynamoDbSagaRepositoryOptions<TSaga>(TableName, Expiration));
            configurator.RegisterLoadSagaRepository<TSaga, DynamoDbSagaRepositoryContextFactory<TSaga>>();
            configurator.RegisterSagaRepository<TSaga, DatabaseContext<TSaga>, SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>,
                DynamoDbSagaRepositoryContextFactory<TSaga>>();
        }
    }
}
