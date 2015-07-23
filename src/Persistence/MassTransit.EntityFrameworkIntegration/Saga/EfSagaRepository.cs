namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Logging;
    using MassTransit.Saga;
    using MassTransit.Pipeline;
    using MassTransit.Util;
    using Mehdime.Entity;

    public class EfSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        private static readonly ILog _log = Logger.Get<EfSagaRepository<TSaga>>();

        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public EfSagaRepository(IDbContextScopeFactory dbContextScopeFactory)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<DbContext>();

                return await dbContext.Set<TSaga>()
                    .Where(query.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .ToListAsync();
            }
        }

        public void Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("sagaRepository");
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<DbContext>();

                var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                var workspace = objectContext.MetadataWorkspace;

                scope.Set(new
                {
                    Persistence = "entityframework",
                    Entities = workspace.GetItems<EntityType>(DataSpace.SSpace).Select(x => x.Name),
                });
            }
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            Guid sagaId = context.CorrelationId.Value;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var dbContext = dbContextScope.DbContexts.Get<DbContext>();


                var sagaInstance = dbContext.Set<TSaga>().SingleOrDefault(x => x.CorrelationId == sagaId);

                if (sagaInstance == null)
                {
                    var missingSagaPipe = new MissingPipe<T>(_dbContextScopeFactory, next);

                    await policy.Missing(context, missingSagaPipe);
                }
                else
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, sagaInstance.CorrelationId, TypeMetadataCache<T>.ShortName);

                    var sagaConsumeContext = new EfSagaConsumeContext<TSaga, T>(_dbContextScopeFactory, context, sagaInstance);

                    await policy.Existing(sagaConsumeContext, next);
                }

                await dbContextScope.SaveChangesAsync();
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                try
                {
                    var dbContext = dbContextScope.DbContexts.Get<DbContext>();

                    var sagaInstances = await dbContext.Set<TSaga>().Where(context.Query.FilterExpression).ToListAsync();

                    if (sagaInstances.Count == 0)
                    {
                        var missingSagaPipe = new MissingPipe<T>(_dbContextScopeFactory, next);
                        await policy.Missing(context, missingSagaPipe);
                    }
                    else
                    {
                        await
                            Task.WhenAll(
                                sagaInstances.Select(instance => SendToInstance(context, policy, instance, next)));
                    }

                    await dbContextScope.SaveChangesAsync();
                }
                catch (SagaException sex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error("Saga Exception Occurred", sex);
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error(
                            string.Format("SAGA:{0} Exception {1}", TypeMetadataCache<TSaga>.ShortName,
                                TypeMetadataCache<T>.ShortName), ex);

                    throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                }
            }
        }

        async Task SendToInstance<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, TSaga instance,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                var sagaConsumeContext = new EfSagaConsumeContext<TSaga, T>(_dbContextScopeFactory, context, instance);

                await policy.Existing(sagaConsumeContext, next);
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
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
            readonly IDbContextScopeFactory _dbContextScopeFactory;

            public MissingPipe(IDbContextScopeFactory dbContextScopeFactory, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _dbContextScopeFactory = dbContextScopeFactory;
                _next = next;
            }

            async void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var dbContext = dbContextScope.DbContexts.Get<DbContext>();

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("SAGA:{0}:{1} Added {2}", TypeMetadataCache<TSaga>.ShortName,
                            context.Saga.CorrelationId,
                            TypeMetadataCache<TMessage>.ShortName);
                    }

                    var proxy = new EfSagaConsumeContext<TSaga, TMessage>(_dbContextScopeFactory, context, context.Saga);

                    await _next.Send(proxy);

                    if (!proxy.IsCompleted)
                        dbContext.Set<TSaga>().Add(context.Saga);

                    await dbContextScope.SaveChangesAsync();
                }
            }
        }
    }
}

