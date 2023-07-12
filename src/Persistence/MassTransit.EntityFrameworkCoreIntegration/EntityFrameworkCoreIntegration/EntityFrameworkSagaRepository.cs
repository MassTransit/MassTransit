namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Data;
    using System.Linq;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Saga;


    public static class EntityFrameworkSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public static ISagaRepository<TSaga> CreateOptimistic(ISagaDbContextFactory<TSaga> dbContextFactory,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            var queryExecutor = new OptimisticLoadQueryExecutor<TSaga>(queryCustomization);
            var lockStrategy = new OptimisticSagaRepositoryLockStrategy<TSaga>(queryExecutor, queryCustomization, IsolationLevel.ReadCommitted);

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

            return new SagaRepository<TSaga>(repositoryFactory, repositoryFactory, repositoryFactory);
        }
    }
}
