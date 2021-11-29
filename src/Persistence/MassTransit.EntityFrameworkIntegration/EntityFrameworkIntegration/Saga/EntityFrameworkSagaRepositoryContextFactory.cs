namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;


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

                var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                var workspace = objectContext.MetadataWorkspace;

                context.Add("entities", workspace.GetItems<EntityType>(DataSpace.SSpace).Select(x => x.Name).ToArray());
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
                await WithinTransaction(dbContext, async () =>
                {
                    using var repositoryContext = new DbContextSagaRepositoryContext<TSaga, T>(dbContext, context, _consumeContextFactory, _lockStrategy);

                    await next.Send(repositoryContext).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
            finally
            {
                _dbContextFactory.Release(dbContext);
            }
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            var dbContext = _dbContextFactory.CreateScoped(context);
            try
            {
                SagaLockContext<TSaga> lockContext = await _lockStrategy.CreateLockContext(dbContext, query, context.CancellationToken).ConfigureAwait(false);

                using var repositoryContext = new DbContextSagaRepositoryContext<TSaga, T>(dbContext, context, _consumeContextFactory, _lockStrategy);

                await WithinTransaction(dbContext, async () =>
                {
                    IList<TSaga> instances = await lockContext.Load().ConfigureAwait(false);

                    var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

                    await next.Send(queryContext).ConfigureAwait(false);
                }).ConfigureAwait(false);
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
                return await WithinTransaction(dbContext, () =>
                {
                    var sagaRepositoryContext = new DbContextSagaRepositoryContext<TSaga>(dbContext, cancellationToken);

                    return asyncMethod(sagaRepositoryContext);
                }).ConfigureAwait(false);
            }
            finally
            {
                _dbContextFactory.Release(dbContext);
            }
        }

        async Task WithinTransaction(DbContext context, Func<Task> callback)
        {
            using var transaction = context.Database.BeginTransaction(_lockStrategy.IsolationLevel);

            void Rollback()
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }
            }

            try
            {
                await callback().ConfigureAwait(false);

                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                Rollback();
                throw;
            }
            catch (DbUpdateException)
            {
                Rollback();
                throw;
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
        }

        async Task<T> WithinTransaction<T>(DbContext context, Func<Task<T>> callback)
        {
            using var transaction = context.Database.BeginTransaction(_lockStrategy.IsolationLevel);

            void Rollback()
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }
            }

            try
            {
                var result = await callback().ConfigureAwait(false);

                transaction.Commit();

                return result;
            }
            catch (DbUpdateConcurrencyException)
            {
                Rollback();
                throw;
            }
            catch (DbUpdateException)
            {
                Rollback();
                throw;
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
        }
    }
}
