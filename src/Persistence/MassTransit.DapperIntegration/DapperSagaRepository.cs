using Microsoft.Extensions.DependencyInjection;

namespace MassTransit
{
    using System;
    using System.Data;
    using DapperIntegration.Saga;
    using Microsoft.Extensions.Options;
    using Saga;


    public static class DapperSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        [Obsolete("Obsolete -- use Create(Action<DapperOptions<TSaga>> configure) instead", false)]
        public static ISagaRepository<TSaga> Create(string connectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var options = new DapperOptions<TSaga>
            {
                ConnectionString = connectionString,
                IsolationLevel = isolationLevel,
                ContextFactoryProvider = _ => (c, t) => new DapperDatabaseContext<TSaga>(c, t),
            };

            var repositoryContextFactory = new DapperSagaRepositoryContextFactory<TSaga>(
                Options.Create(options),
                consumeContextFactory,
                null
            );

            return new SagaRepository<TSaga>(repositoryContextFactory, repositoryContextFactory, repositoryContextFactory);
        }

        public static ISagaRepository<TSaga> Create(Action<DapperOptions<TSaga>> configure = null, IServiceProvider serviceProvider = null)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var options = new DapperOptions<TSaga>
            {
                ContextFactoryProvider = _ => (c, t) => new DapperDatabaseContext<TSaga>(c, t),
            };

            configure?.Invoke(options);

            var repositoryContextFactory = new DapperSagaRepositoryContextFactory<TSaga>(
                Options.Create(options),
                consumeContextFactory,
                serviceProvider
            );

            return new SagaRepository<TSaga>(repositoryContextFactory, repositoryContextFactory, repositoryContextFactory);
        }
    }
}
