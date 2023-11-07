namespace MassTransit.Configuration.Configuration
{
    using System;
    using System.Collections.Generic;
    using Cassandra;
    using CassandraDbIntegration.Saga;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;


    public class CassandraDbSagaRepositoryConfigurator<TSaga> :
        ICassandraDbSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISagaVersion
    {
        Func<IServiceProvider, ISession> _contextFactory;

        public CassandraDbSagaRepositoryConfigurator()
        {
        }

        public string TableName { get; set; }

        public void ContextFactory(Func<ISession> contextFactory)
        {
            _contextFactory = provider => contextFactory();
        }

        public void ContextFactory(Func<IServiceProvider, ISession> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_contextFactory == null)
                yield return this.Failure("ContextFactory", "must be specified");
            if (string.IsNullOrWhiteSpace(TableName))
                yield return this.Failure("TableName", "must be specified");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, ISagaVersion
        {
            configurator.TryAddSingleton(_contextFactory);
            configurator.TryAddSingleton(new CassandraDbSagaRepositoryOptions<T>());
            configurator.RegisterLoadSagaRepository<T, CassandraDbSagaRepositoryContextFactory<T>>();
            configurator
                .RegisterSagaRepository<T, DatabaseContext<T>, SagaConsumeContextFactory<DatabaseContext<T>, T>, CassandraDbSagaRepositoryContextFactory<T>>();
        }
    }
}
