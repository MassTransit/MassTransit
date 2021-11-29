namespace MassTransit.EntityFrameworkIntegration
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using MassTransit.Saga;
    using Saga;


    public static class EntityFrameworkSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public static ISagaRepository<TSaga> CreateOptimistic(ISagaDbContextFactory<TSaga> dbContextFactory,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            ILoadQueryProvider<TSaga> queryProvider = new DefaultSagaLoadQueryProvider<TSaga>();
            if (queryCustomization != null)
                queryProvider = new CustomSagaLoadQueryProvider<TSaga>(queryProvider, queryCustomization);

            var queryExecutor = new OptimisticLoadQueryExecutor<TSaga>(queryProvider);
            var lockStrategy = new OptimisticSagaRepositoryLockStrategy<TSaga>(queryProvider, queryExecutor, IsolationLevel.ReadCommitted);

            return CreateRepository(dbContextFactory, lockStrategy);
        }

        public static ISagaRepository<TSaga> CreateOptimistic(Func<DbContext> dbContextFactory,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            return CreateOptimistic(new DelegateSagaDbContextFactory<TSaga>(dbContextFactory), queryCustomization);
        }

        public static ISagaRepository<TSaga> CreatePessimistic(ISagaDbContextFactory<TSaga> dbContextFactory,
            ILockStatementProvider lockStatementProvider = null,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            var statementProvider = lockStatementProvider ?? new SqlServerLockStatementProvider();

            var queryExecutor = new PessimisticLoadQueryExecutor<TSaga>(statementProvider, queryCustomization);
            var lockStrategy = new PessimisticSagaRepositoryLockStrategy<TSaga>(queryExecutor, IsolationLevel.Serializable);

            return CreateRepository(dbContextFactory, lockStrategy);
        }

        public static ISagaRepository<TSaga> CreatePessimistic(Func<DbContext> dbContextFactory, ILockStatementProvider lockStatementProvider = null,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            return CreatePessimistic(new DelegateSagaDbContextFactory<TSaga>(dbContextFactory), lockStatementProvider, queryCustomization);
        }

        static ISagaRepository<TSaga> CreateRepository(ISagaDbContextFactory<TSaga> dbContextFactory, ISagaRepositoryLockStrategy<TSaga> lockStrategy)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<DbContext, TSaga>();

            var repositoryFactory =
                new EntityFrameworkSagaRepositoryContextFactory<TSaga>(dbContextFactory, consumeContextFactory, lockStrategy);

            return new SagaRepository<TSaga>(repositoryFactory);
        }
    }
}
