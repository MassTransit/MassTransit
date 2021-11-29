namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EntityFrameworkCoreIntegration;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;


    public class EntityFrameworkSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<DbContext, TSaga> _consumeContextFactory;
        readonly ISagaDbContextFactory<TSaga> _dbContextFactory;
        readonly ISagaRepositoryLockStrategy<TSaga> _lockStrategy;

        public EntityFrameworkSagaRepositoryContextFactory(ISagaDbContextFactory<TSaga> dbContextFactory,
            ISagaConsumeContextFactory<DbContext, TSaga> consumeContextFactory, ISagaRepositoryLockStrategy<TSaga> lockStrategy)
        {
            _dbContextFactory = dbContextFactory;
            _consumeContextFactory = consumeContextFactory;
            _lockStrategy = lockStrategy;
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
                Task Send()
                {
                    return WithinTransaction(dbContext, context.CancellationToken, async () =>
                    {
                        using var repositoryContext = new DbContextSagaRepositoryContext<TSaga, T>(dbContext, context, _consumeContextFactory, _lockStrategy);

                        await next.Send(repositoryContext).ConfigureAwait(false);
                    });
                }

                var executionStrategy = dbContext.Database.CreateExecutionStrategy();
                if (executionStrategy is ExecutionStrategy)
                    await executionStrategy.ExecuteAsync(Send).ConfigureAwait(false);
                else
                    await Send().ConfigureAwait(false);
            }
            finally
            {
                await _dbContextFactory.ReleaseAsync(dbContext).ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            var dbContext = _dbContextFactory.CreateScoped(context);
            try
            {
                async Task Send()
                {
                    SagaLockContext<TSaga> lockContext =
                        await _lockStrategy.CreateLockContext(dbContext, query, context.CancellationToken).ConfigureAwait(false);

                    using var repositoryContext = new DbContextSagaRepositoryContext<TSaga, T>(dbContext, context, _consumeContextFactory, _lockStrategy);

                    await WithinTransaction(dbContext, context.CancellationToken, async () =>
                    {
                        IList<TSaga> instances = await lockContext.Load().ConfigureAwait(false);

                        var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

                        await next.Send(queryContext).ConfigureAwait(false);
                    }).ConfigureAwait(false);
                }

                var executionStrategy = dbContext.Database.CreateExecutionStrategy();
                if (executionStrategy is ExecutionStrategy)
                    await executionStrategy.ExecuteAsync(Send).ConfigureAwait(false);
                else
                    await Send().ConfigureAwait(false);
            }
            finally
            {
                await _dbContextFactory.ReleaseAsync(dbContext).ConfigureAwait(false);
            }
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var dbContext = _dbContextFactory.Create();
            try
            {
                Task<T> Send()
                {
                    return WithinTransaction(dbContext, cancellationToken, () =>
                    {
                        var sagaRepositoryContext = new DbContextSagaRepositoryContext<TSaga>(dbContext, cancellationToken);

                        return asyncMethod(sagaRepositoryContext);
                    });
                }

                var executionStrategy = dbContext.Database.CreateExecutionStrategy();
                if (executionStrategy is ExecutionStrategy)
                    return await executionStrategy.ExecuteAsync(Send).ConfigureAwait(false);
                else
                    return await Send().ConfigureAwait(false);
            }
            finally
            {
                await _dbContextFactory.ReleaseAsync(dbContext).ConfigureAwait(false);
            }
        }

        Task WithinTransaction(DbContext context, CancellationToken cancellationToken, Func<Task> callback)
        {
            async Task<bool> Create()
            {
                await callback().ConfigureAwait(false);
                return true;
            }

            return WithinTransaction(context, cancellationToken, Create);
        }

        async Task<T> WithinTransaction<T>(DbContext context, CancellationToken cancellationToken, Func<Task<T>> callback)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(_lockStrategy.IsolationLevel, cancellationToken).ConfigureAwait(false);

            static async Task Rollback(IDbContextTransaction transaction)
            {
                try
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }
            }

            try
            {
                var result = await callback().ConfigureAwait(false);

                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

                return result;
            }
            catch (DbUpdateConcurrencyException)
            {
                await Rollback(transaction).ConfigureAwait(false);
                throw;
            }
            catch (DbUpdateException)
            {
                await Rollback(transaction).ConfigureAwait(false);
                throw;
            }
            catch (Exception)
            {
                await Rollback(transaction).ConfigureAwait(false);
                throw;
            }
        }
    }
}
