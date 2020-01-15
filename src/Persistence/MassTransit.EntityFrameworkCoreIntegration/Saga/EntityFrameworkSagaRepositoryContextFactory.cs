namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public class EntityFrameworkSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaDbContextFactory<TSaga> _dbContextFactory;
        readonly ISagaConsumeContextFactory<DbContext, TSaga> _sagaConsumeContextFactory;
        readonly IsolationLevel _isolationLevel;

        public EntityFrameworkSagaRepositoryContextFactory(ISagaDbContextFactory<TSaga> dbContextFactory,
            ISagaConsumeContextFactory<DbContext, TSaga> sagaConsumeContextFactory, IsolationLevel isolationLevel)
        {
            _dbContextFactory = dbContextFactory;
            _sagaConsumeContextFactory = sagaConsumeContextFactory;
            _isolationLevel = isolationLevel;
        }

        public void Probe(ProbeContext context)
        {
            var dbContext = _dbContextFactory.Create();
            try
            {
                context.Add("persistence", "entity-framework");
                context.Add("entities", dbContext.Model.GetEntityTypes().Select(type => type.Name).ToArray());
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var dbContext = _dbContextFactory.CreateScoped(context);
            try
            {
                async Task Send()
                {
                    using var transaction =
                        await dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken).ConfigureAwait(false);

                    var sagaRepositoryContext = new DbContextSagaRepositoryContext<TSaga, T>(dbContext, context, _sagaConsumeContextFactory);

                    await next.Send(sagaRepositoryContext).ConfigureAwait(false);

                    transaction.Commit();
                }

                var executionStrategy = dbContext.Database.CreateExecutionStrategy();
                if (executionStrategy is SqlServerRetryingExecutionStrategy)
                {
                    await executionStrategy.ExecuteAsync(Send).ConfigureAwait(false);
                }
                else
                {
                    await Send().ConfigureAwait(false);
                }
            }
            finally
            {
                _dbContextFactory.Release(dbContext);
            }
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var dbContext = _dbContextFactory.Create();
            try
            {
                async Task<T> Send()
                {
                    using var transaction =
                        await dbContext.Database.BeginTransactionAsync(_isolationLevel, cancellationToken).ConfigureAwait(false);

                    var sagaRepositoryContext = new DbContextSagaRepositoryContext<TSaga>(dbContext, cancellationToken);

                    var result = await asyncMethod(sagaRepositoryContext).ConfigureAwait(false);

                    transaction.Commit();

                    return result;
                }

                var executionStrategy = dbContext.Database.CreateExecutionStrategy();
                if (executionStrategy is SqlServerRetryingExecutionStrategy)
                {
                    return await executionStrategy.ExecuteAsync(Send).ConfigureAwait(false);
                }
                else
                {
                    return await Send().ConfigureAwait(false);
                }
            }
            finally
            {
                _dbContextFactory.Release(dbContext);
            }
        }
    }
}
