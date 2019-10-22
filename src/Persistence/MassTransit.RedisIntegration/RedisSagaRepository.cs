namespace MassTransit.RedisIntegration
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;
    using StackExchange.Redis;


    public class RedisSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IRetrieveSagaFromRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly Func<IDatabase> _redisDbFactory;
        readonly bool _optimistic;
        readonly TimeSpan _lockTimeout;
        readonly TimeSpan _lockRetryTimeout;

        public RedisSagaRepository(Func<IDatabase> redisDbFactory, bool optimistic = true, TimeSpan? lockTimeout = null, TimeSpan? lockRetryTimeout = null, string keyPrefix = "")
        {
            _redisDbFactory = redisDbFactory;
            _optimistic = optimistic;

            _lockTimeout = lockTimeout ?? TimeSpan.FromSeconds(30);
            _lockRetryTimeout = lockRetryTimeout ?? TimeSpan.FromSeconds(30);
            DatabaseExtensions.SetKeyPrefix(keyPrefix);
        }

        async Task<TSaga> IRetrieveSagaFromRepository<TSaga>.GetSaga(Guid correlationId)
        {
            return await _redisDbFactory().As<TSaga>().Get(correlationId).ConfigureAwait(false);
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var sagaId = context.CorrelationId.Value;
            var db = _redisDbFactory();

            ITypedDatabase<TSaga> sagas = db.As<TSaga>();

            IAsyncDisposable pessimisticLock = null;

            if (!_optimistic)
                pessimisticLock = await db.AcquireLockAsync(sagaId, _lockTimeout, _lockRetryTimeout).ConfigureAwait(false);

            try
            {
                if (policy.PreInsertInstance(context, out var instance))
                    await PreInsertSagaInstance(sagas, context, instance).ConfigureAwait(false);

                if (instance == null)
                    instance = await sagas.Get(sagaId).ConfigureAwait(false);

                if (instance == null)
                {
                    var missingSagaPipe = new MissingPipe<T>(sagas, next);

                    await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                }
                else
                    await SendToInstance(sagas, context, policy, next, instance).ConfigureAwait(false);
            }
            finally
            {
                if (!_optimistic)
                    await pessimisticLock.DisposeAsync().ConfigureAwait(false);
            }
        }

        Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            throw new NotImplementedByDesignException("Redis saga repository does not support queries");
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            scope.Set(new {Persistence = "redis"});
        }

        async Task SendToInstance<T>(ITypedDatabase<TSaga> sagas, ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next, TSaga instance)
            where T : class
        {
            try
            {
                var sagaConsumeContext = new RedisSagaConsumeContext<TSaga, T>(sagas, context, instance);

                sagaConsumeContext.LogUsed();

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                    await UpdateRedisSaga<T>(sagas, instance).ConfigureAwait(false);
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

        async Task PreInsertSagaInstance<T>(ITypedDatabase<TSaga> sagas, ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            try
            {
                await sagas.Put(instance.CorrelationId, instance).ConfigureAwait(false);

                context.LogInsert(this, instance.CorrelationId);
            }
            catch (Exception ex)
            {
                context.LogInsertFault(this, ex, instance.CorrelationId);
            }
        }

        async Task UpdateRedisSaga<T>(ITypedDatabase<TSaga> sagas, TSaga instance)
            where T : class
        {
            IAsyncDisposable updateLock = null;

            if (_optimistic)
                updateLock = await sagas.Database.AcquireLockAsync(instance.CorrelationId, _lockTimeout, _lockRetryTimeout).ConfigureAwait(false);

            try
            {
                instance.Version++;
                var old = await sagas.Get(instance.CorrelationId).ConfigureAwait(false);

                if (old.Version >= instance.Version)
                    throw new RedisSagaConcurrencyException("Saga version conflict", typeof(TSaga), typeof(T), instance.CorrelationId);

                await sagas.Put(instance.CorrelationId, instance).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Failed to updated saga", typeof(TSaga), typeof(T), instance.CorrelationId, exception);
            }
            finally
            {
                if (_optimistic)
                    await updateLock.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
            readonly ITypedDatabase<TSaga> _sagas;

            public MissingPipe(ITypedDatabase<TSaga> sagas, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _sagas = sagas;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                SagaConsumeContext<TSaga, TMessage> proxy = new RedisSagaConsumeContext<TSaga, TMessage>(_sagas, context, context.Saga);

                proxy.LogAdded();

                await _next.Send(proxy).ConfigureAwait(false);

                if (!proxy.IsCompleted)
                    await _sagas.Put(context.Saga.CorrelationId, context.Saga).ConfigureAwait(false);
            }
        }
    }
}
