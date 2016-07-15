// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Saga;
    using Pipeline;
    using Util;


    public class EntityFrameworkSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<EntityFrameworkSagaRepository<TSaga>>();
        readonly SagaDbContextFactory _sagaDbContextFactory;
        readonly IsolationLevel _isolationLevel;

        public EntityFrameworkSagaRepository(SagaDbContextFactory sagaDbContextFactory, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            _sagaDbContextFactory = sagaDbContextFactory;
            _isolationLevel = isolationLevel;
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
            ProbeContext scope = context.CreateScope("sagaRepository");
            using (var dbContext = _sagaDbContextFactory())
            {
                var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                var workspace = objectContext.MetadataWorkspace;

                scope.Set(new
                {
                    Persistence = "entityFramework",
                    Entities = workspace.GetItems<EntityType>(DataSpace.SSpace).Select(x => x.Name)
                });
            }
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            Guid sagaId = context.CorrelationId.Value;

            using (DbContext dbContext = _sagaDbContextFactory())
            using(var transaction = dbContext.Database.BeginTransaction(_isolationLevel))
            {
                bool inserted = false;

                TSaga instance;
                if (policy.PreInsertInstance(context, out instance))
                {
                    inserted = await PreInsertSagaInstance<T>(dbContext, instance, context.CancellationToken).ConfigureAwait(false);
                }

                try
                {
                    if (instance == null)
                        instance = dbContext.Set<TSaga>().SingleOrDefault(x => x.CorrelationId == sagaId);
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

                    await dbContext.SaveChangesAsync().ConfigureAwait(false);

                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    var baseException = ex.GetBaseException() as SqlException;
                    if(baseException != null && baseException.Number == 1205)
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
                catch (Exception)
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
            }
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

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            using (var dbContext = _sagaDbContextFactory())
            using (var transaction = dbContext.Database.BeginTransaction(_isolationLevel))
            {
                try
                {
                    var sagaInstances = await dbContext.Set<TSaga>().Where(context.Query.FilterExpression).ToListAsync().ConfigureAwait(false);
                    if (sagaInstances.Count == 0)
                    {
                        var missingSagaPipe = new MissingPipe<T>(dbContext, next);

                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (var instance in sagaInstances)
                        {
                            await SendToInstance(context, dbContext, policy, instance, next).ConfigureAwait(false);
                        }
                    }

                    await dbContext.SaveChangesAsync().ConfigureAwait(false);

                    transaction.Commit();
                }
                catch (SagaException sex)
                {
                    transaction.Rollback();
                    if (_log.IsErrorEnabled)
                        _log.Error("Saga Exception Occurred", sex);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (_log.IsErrorEnabled)
                        _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", ex);

                    throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                }
            }
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

                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}