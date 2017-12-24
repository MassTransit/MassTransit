// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using GreenPipes;

    using MassTransit.Logging;
    using MassTransit.Saga;
    using MassTransit.Util;

    using Microsoft.EntityFrameworkCore;


    public class EntityFrameworkSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<EntityFrameworkSagaRepository<TSaga>>();
        readonly IsolationLevel _isolationLevel;
        readonly Func<DbContext> _sagaDbContextFactory;
        readonly bool _optimistic;
        readonly Func<IQueryable<TSaga>, IQueryable<TSaga>> _queryCustomization;

        public EntityFrameworkSagaRepository(
            Func<DbContext> sagaDbContextFactory, 
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, 
            bool optimistic = false,
            Func<IQueryable<TSaga>, IQueryable<TSaga>> queryCustomization = null)
        {
            _sagaDbContextFactory = sagaDbContextFactory;
            _isolationLevel = isolationLevel;
            _optimistic = optimistic;
            _queryCustomization = queryCustomization;
        }

        async Task<IEnumerable<Guid>> IQuerySagaRepository<TSaga>.Find(ISagaQuery<TSaga> query)
        {
            using (var dbContext = _sagaDbContextFactory())
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
            using (var dbContext = _sagaDbContextFactory())
            {
                scope.Set(new
                {
                    Persistence = "entityFramework",
                    Entities = dbContext.Model.GetEntityTypes().Select(type => type.Name)
                });
            }
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var sagaId = context.CorrelationId.Value;

            using (var dbContext = _sagaDbContextFactory())
            using (var transaction = await dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken).ConfigureAwait(false))
            {
                if (!_optimistic)
                {
                    // Hack for locking row for the duration of the transaction.
                    var tableName = dbContext.GetTableName<TSaga>();
                    await dbContext.Database.ExecuteSqlCommandAsync(
                        $"select 1 from {tableName} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @p0",
                        new object[] { sagaId },
                        context.CancellationToken).ConfigureAwait(false);
                }

                var inserted = false;

                TSaga instance;
                if (policy.PreInsertInstance(context, out instance))
                {
                    inserted = await PreInsertSagaInstance<T>(dbContext, instance, context.CancellationToken).ConfigureAwait(false);
                }

                try
                {
                    if (instance == null)
                    {
                        instance =
                            await QuerySagas<T>(dbContext).SingleOrDefaultAsync(x => x.CorrelationId == sagaId, context.CancellationToken).ConfigureAwait(false);
                    }
                    if (instance == null)
                    {
                        var missingSagaPipe = new MissingPipe<T>(dbContext, next);

                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }
                    else
                    {
                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId,
                                TypeMetadataCache<T>.ShortName);
                        }

                        var sagaConsumeContext = new EntityFrameworkSagaConsumeContext<TSaga, T>(dbContext, context, instance);

                        await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
                    }

                    await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception innerException)
                    {
                        if (_log.IsWarnEnabled)
                            _log.Warn("The transaction rollback failed", innerException);
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
                        if (_log.IsErrorEnabled)
                            _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", ex);

                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            if (_log.IsWarnEnabled)
                                _log.Warn("The transaction rollback failed", innerException);
                        }
                    }

                    throw;
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", ex);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception innerException)
                    {
                        if (_log.IsWarnEnabled)
                            _log.Warn("The transaction rollback failed", innerException);
                    }
                    throw;
                }
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            using (var dbContext = _sagaDbContextFactory())
            {
                // We just get the correlation ids related to our Filter.
                // We do this outside of the transaction to make sure we don't create a range lock.
                List<Guid> correlationIds = await dbContext.Set<TSaga>()
                    .Where(context.Query.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .ToListAsync(context.CancellationToken)
                    .ConfigureAwait(false);

                using (var transaction = 
                    await dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        var missingCorrelationIds = new List<Guid>();
                        if (correlationIds.Any())
                        {
                            var tableName = dbContext.GetTableName<TSaga>();
                            foreach (var correlationId in correlationIds)
                            {
                                if (!_optimistic)
                                {
                                    // Hack for locking row for the duration of the transaction. 
                                    // We only lock one at a time, since we don't want an accidental range lock.
                                    await
                                        dbContext.Database.ExecuteSqlCommandAsync(
                                            $"select 2 from {tableName} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @p0",
                                            new object[] {correlationId}, context.CancellationToken).ConfigureAwait(false);
                                }

                                var instance = 
                                    await QuerySagas<T>(dbContext).SingleOrDefaultAsync(x => x.CorrelationId == correlationId, context.CancellationToken)
                                        .ConfigureAwait(false);

                                if (instance != null)
                                {
                                    await SendToInstance(context, dbContext, policy, instance, next).ConfigureAwait(false);
                                }
                                else
                                {
                                    missingCorrelationIds.Add(correlationId);
                                }
                            }
                        }

                        // If no sagas are found or all are missing
                        if (correlationIds.Count == missingCorrelationIds.Count)
                        {
                            var missingSagaPipe = new MissingPipe<T>(dbContext, next);

                            await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                        }

                        await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                        transaction.Commit();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            if (_log.IsWarnEnabled)
                                _log.Warn("The transaction rollback failed", innerException);
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
                                if (_log.IsWarnEnabled)
                                    _log.Warn("The transaction rollback failed", innerException);
                            }
                        }

                        throw;
                    }
                    catch (SagaException sex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", sex);

                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception innerException)
                        {
                            if (_log.IsWarnEnabled)
                                _log.Warn("The transaction rollback failed", innerException);
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
                            if (_log.IsWarnEnabled)
                                _log.Warn("The transaction rollback failed", innerException);
                        }

                        if (_log.IsErrorEnabled)
                            _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", ex);

                        throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                    }
                }
            }
        }

        static bool IsDeadlockException(Exception exception)
        {
            var baseException = exception.GetBaseException() as SqlException;

            return baseException != null && baseException.Number == 1205;
        }

        static async Task<bool> PreInsertSagaInstance<T>(DbContext dbContext, TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                dbContext.Set<TSaga>().Add(instance);
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                _log.DebugFormat("SAGA:{0}:{1} Insert {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId,
                    TypeMetadataCache<T>.ShortName);

                return true;
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Dupe {2} - {3}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId,
                        TypeMetadataCache<T>.ShortName, ex.Message);
                }
            }

            return false;
        }

        async Task SendToInstance<T>(SagaQueryConsumeContext<TSaga, T> context, DbContext dbContext, ISagaPolicy<TSaga, T> policy, TSaga instance,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                var sagaConsumeContext = new EntityFrameworkSagaConsumeContext<TSaga, T>(dbContext, context, instance);

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
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Added {2}", TypeMetadataCache<TSaga>.ShortName,
                        context.Saga.CorrelationId,
                        TypeMetadataCache<TMessage>.ShortName);
                }

                var proxy = new EntityFrameworkSagaConsumeContext<TSaga, TMessage>(_dbContext, context, context.Saga, false);

                await _next.Send(proxy).ConfigureAwait(false);

                if (!proxy.IsCompleted)
                    _dbContext.Set<TSaga>().Add(context.Saga);

                await _dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);
            }
        }


        IQueryable<TSaga> QuerySagas<T>(DbContext dbContext)
        {
            IQueryable<TSaga> query = dbContext.Set<TSaga>();
            
            if (_queryCustomization != null)
            {
                query = _queryCustomization(query);
            }
            
            return query;
        }
    }
}