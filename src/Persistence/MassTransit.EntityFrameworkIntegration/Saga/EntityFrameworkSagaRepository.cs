namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Saga;


    public class EntityFrameworkSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IsolationLevel _isolationLevel;
        readonly ISagaDbContextFactory<TSaga> _sagaDbContextFactory;
        readonly Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;
        readonly IRawSqlLockStatements _rawSqlLockStatements;

        public EntityFrameworkSagaRepository(ISagaDbContextFactory<TSaga> sagaDbContextFactory,
            IsolationLevel isolationLevel,
            IRawSqlLockStatements rawSqlLockStatements = null,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            _sagaDbContextFactory = sagaDbContextFactory;
            _isolationLevel = isolationLevel;
            _rawSqlLockStatements = rawSqlLockStatements;
            _queryCustomization = queryCustomization;
        }

        public static EntityFrameworkSagaRepository<TSaga> CreateOptimistic(ISagaDbContextFactory<TSaga> sagaDbContextFactory,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            return new EntityFrameworkSagaRepository<TSaga>(sagaDbContextFactory, IsolationLevel.ReadCommitted, null, queryCustomization);
        }

        public static EntityFrameworkSagaRepository<TSaga> CreateOptimistic(Func<DbContext> sagaDbContextFactory,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            return CreateOptimistic(new DelegateSagaDbContextFactory<TSaga>(sagaDbContextFactory), queryCustomization);
        }

        public static EntityFrameworkSagaRepository<TSaga> CreatePessimistic(ISagaDbContextFactory<TSaga> sagaDbContextFactory,
            IRawSqlLockStatements rawSqlLockStatements = null,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            return new EntityFrameworkSagaRepository<TSaga>(sagaDbContextFactory, IsolationLevel.Serializable,
                rawSqlLockStatements ?? new MsSqlLockStatements(), queryCustomization);
        }

        public static EntityFrameworkSagaRepository<TSaga> CreatePessimistic(Func<DbContext> sagaDbContextFactory,
            IRawSqlLockStatements rawSqlLockStatements = null,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            return CreatePessimistic(new DelegateSagaDbContextFactory<TSaga>(sagaDbContextFactory), rawSqlLockStatements, queryCustomization);
        }

        async Task<IEnumerable<Guid>> IQuerySagaRepository<TSaga>.Find(ISagaQuery<TSaga> query)
        {
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                return await dbContext.Set<TSaga>()
                    .Where(query.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .ToListAsync().ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");

            var dbContext = _sagaDbContextFactory.Create();
            try
            {
                var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                var workspace = objectContext.MetadataWorkspace;

                scope.Set(new
                {
                    Persistence = "entityFramework",
                    Entities = workspace.GetItems<EntityType>(DataSpace.SSpace).Select(x => x.Name)
                });
            }
            finally
            {
                _sagaDbContextFactory.Release(dbContext);
            }
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var sagaId = context.CorrelationId.Value;

            var dbContext = _sagaDbContextFactory.CreateScoped(context);
            try
            {
                using (var transaction = dbContext.Database.BeginTransaction(_isolationLevel))
                {
                    if (policy.PreInsertInstance(context, out var instance))
                    {
                        var inserted = await PreInsertSagaInstance(dbContext, context, instance).ConfigureAwait(false);
                        if (!inserted)
                            instance = null; // Reset this back to null if the insert failed. We will use the MissingPipe to create instead
                    }

                    try
                    {
                        if (instance == null)
                        {
                            // Only perform this additional DB Call for pessimistic concurrency
                            if (_rawSqlLockStatements != null)
                            {
                                var rowLockQuery = _rawSqlLockStatements.GetRowLockStatement<TSaga>(dbContext);
                                await dbContext.Database.ExecuteSqlCommandAsync(rowLockQuery, context.CancellationToken, sagaId).ConfigureAwait(false);
                            }

                            instance = await QuerySagas(dbContext)
                                .SingleOrDefaultAsync(x => x.CorrelationId == sagaId, context.CancellationToken)
                                .ConfigureAwait(false);
                        }

                        if (instance == null)
                        {
                            var missingSagaPipe = new MissingPipe<T>(dbContext, next);

                            await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                        }
                        else
                        {
                            var sagaConsumeContext = new EntityFrameworkSagaConsumeContext<TSaga, T>(dbContext, context, instance);

                            sagaConsumeContext.LogUsed();

                            await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
                        }

                        await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                        transaction.Commit();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                        }

                        throw;
                    }
                    catch (DbUpdateException ex)
                    {
                        if (IsDeadlockException(ex))
                        {
                            // deadlock, no need to rollback
                        }
                        else
                        {
                            context.LogFault(this, ex);

                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception innerException)
                            {
                                LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                            }
                        }

                        throw;
                    }
                    catch (Exception ex)
                    {
                        context.LogFault(this, ex);

                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                        }

                        throw;
                    }
                }
            }
            finally
            {
                _sagaDbContextFactory.Release(dbContext);
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            var dbContext = _sagaDbContextFactory.CreateScoped(context);
            try
            {
                List<Guid> nonTrackedInstances = null;

                // Only perform this additional DB Call for pessimistic concurrency
                if (_rawSqlLockStatements != null)
                {
                    // We just get the correlation ids related to our Filter.
                    // We do this outside of the transaction to make sure we don't create a range lock.
                    nonTrackedInstances = await dbContext.Set<TSaga>()
                        .AsNoTracking()
                        .Where(context.Query.FilterExpression)
                        .Select(x => x.CorrelationId)
                        .ToListAsync(context.CancellationToken)
                        .ConfigureAwait(false);
                }

                using (var transaction = dbContext.Database.BeginTransaction(_isolationLevel))
                {
                    try
                    {
                        // Simple path for Optimistic Concurrency
                        if (_rawSqlLockStatements == null)
                        {
                            var instances = await QuerySagas(dbContext)
                                .Where(context.Query.FilterExpression)
                                .ToListAsync(context.CancellationToken)
                                .ConfigureAwait(false);

                            if (!instances.Any())
                            {
                                var missingSagaPipe = new MissingPipe<T>(dbContext, next);
                                await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                            }
                            else
                                await Task.WhenAll(instances.Select(instance => SendToInstance(context, dbContext, policy, instance, next)))
                                    .ConfigureAwait(false);
                        }
                        // Pessimistic Concurrency
                        else
                        {
                            var rowLockQuery = _rawSqlLockStatements.GetRowLockStatement<TSaga>(dbContext);

                            var missingCorrelationIds = new List<Guid>();

                            if (nonTrackedInstances?.Any() == true)
                            {
                                var foundInstances = new List<Task>();

                                foreach (var nonTrackedInstance in nonTrackedInstances)
                                {
                                    // Hack for locking row for the duration of the transaction.
                                    // We only lock one at a time, since we don't want an accidental range lock.
                                    await dbContext.Database.ExecuteSqlCommandAsync(rowLockQuery, context.CancellationToken, nonTrackedInstance)
                                        .ConfigureAwait(false);

                                    var instance = await QuerySagas(dbContext)
                                        .SingleOrDefaultAsync(x => x.CorrelationId == nonTrackedInstance, context.CancellationToken)
                                        .ConfigureAwait(false);

                                    if (instance != null)
                                        foundInstances.Add(SendToInstance(context, dbContext, policy, instance, next));
                                    else
                                        missingCorrelationIds.Add(nonTrackedInstance);
                                }

                                if (foundInstances.Any())
                                    await Task.WhenAll(foundInstances).ConfigureAwait(false);
                            }

                            // If no sagas are found or all are missing
                            if (nonTrackedInstances.Count == missingCorrelationIds.Count)
                            {
                                var missingSagaPipe = new MissingPipe<T>(dbContext, next);

                                await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                            }
                        }

                        await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                        transaction.Commit();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                        }

                        throw;
                    }
                    catch (DbUpdateException ex)
                    {
                        if (IsDeadlockException(ex))
                        {
                            // deadlock, no need to rollback
                        }
                        else
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

                        throw;
                    }
                    catch (SagaException sex)
                    {
                        context.LogFault(sex);

                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                        }

                        throw;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                        }

                        context.LogFault(ex);

                        throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                    }
                }
            }
            finally
            {
                _sagaDbContextFactory.Release(dbContext);
            }
        }

        static bool IsDeadlockException(DataException exception)
        {
            var baseException = exception.GetBaseException() as SqlException;

            return baseException != null && baseException.Number == 1205;
        }

        async Task<bool> PreInsertSagaInstance<T>(DbContext dbContext, ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            TSaga entity = null;

            try
            {
                entity = dbContext.Set<TSaga>().Add(instance);
                await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                context.LogInsert(this, instance.CorrelationId);

                return true;
            }
            catch (Exception ex)
            {
                // Because we will still be using the same dbContext, we need to reset the entry we just tried to pre-insert (likely a duplicate), so
                // on the next save changes (which is the update), it will pass.
                // see here for details: https://www.davideguida.com/how-to-reset-the-entities-state-on-a-entity-framework-db-context/
                dbContext.Entry(entity).State = EntityState.Detached;

                context.LogInsertFault(this, ex, instance.CorrelationId);
            }

            return false;
        }

        async Task SendToInstance<T>(SagaQueryConsumeContext<TSaga, T> context, DbContext dbContext, ISagaPolicy<TSaga, T> policy, TSaga instance,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                var sagaConsumeContext = new EntityFrameworkSagaConsumeContext<TSaga, T>(dbContext, context, instance);

                sagaConsumeContext.LogUsed();

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
            }
            catch (SagaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
            }
        }

        IQueryable<TSaga> QuerySagas(DbContext dbContext)
        {
            IQueryable<TSaga> query = dbContext.Set<TSaga>();

            if (_queryCustomization != null)
                query = _queryCustomization(query);

            return query;
        }


        /// <summary>
        /// Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly DbContext _dbContext;
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;

            public MissingPipe(DbContext dbContext, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _dbContext = dbContext;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                var proxy = new EntityFrameworkSagaConsumeContext<TSaga, TMessage>(_dbContext, context, context.Saga, false);

                proxy.LogAdded();

                await _next.Send(proxy).ConfigureAwait(false);

                if (!proxy.IsCompleted)
                    _dbContext.Set<TSaga>().Add(context.Saga);

                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
