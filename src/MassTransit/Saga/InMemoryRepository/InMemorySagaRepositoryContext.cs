namespace MassTransit.Saga.InMemoryRepository
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class InMemorySagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>,
        IDisposable
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IndexedSagaDictionary<TSaga> _sagas;
        readonly ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga> _factory;
        readonly ConsumeContext<TMessage> _context;
        bool _sagasLocked;

        public InMemorySagaRepositoryContext(IndexedSagaDictionary<TSaga> sagas, ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga> factory,
            ConsumeContext<TMessage> context)
            : base(context)
        {
            _sagas = sagas;
            _factory = factory;
            _context = context;
            _sagasLocked = true;
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            if (_sagasLocked)
            {
                var consumeContext = await _factory.CreateSagaConsumeContext(_sagas, _context, instance, SagaConsumeContextMode.Add).ConfigureAwait(false);

                _sagas.Release();
                _sagasLocked = false;

                return consumeContext;
            }

            await _sagas.MarkInUse(_context.CancellationToken).ConfigureAwait(false);
            try
            {
                return await _factory.CreateSagaConsumeContext(_sagas, _context, instance, SagaConsumeContextMode.Add).ConfigureAwait(false);
            }
            finally
            {
                _sagas.Release();
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            if (_sagasLocked)
            {
                if (_sagas[instance.CorrelationId] != null)
                    return default;

                return await _factory.CreateSagaConsumeContext(_sagas, _context, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }

            await _sagas.MarkInUse(_context.CancellationToken).ConfigureAwait(false);
            try
            {
                if (_sagas[instance.CorrelationId] != null)
                    return default;

                return await _factory.CreateSagaConsumeContext(_sagas, _context, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            finally
            {
                _sagas.Release();
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            SagaInstance<TSaga> saga;
            if (_sagasLocked)
            {
                saga = _sagas[correlationId];
                if (saga == null)
                    return default;

                if (saga.IsRemoved)
                {
                    saga.Release();
                    return default;
                }

                _sagas.Release();
                _sagasLocked = false;
            }
            else
            {
                await _sagas.MarkInUse(_context.CancellationToken).ConfigureAwait(false);
                try
                {
                    saga = _sagas[correlationId];

                    if (saga == null)
                        return default;

                    if (saga.IsRemoved)
                    {
                        saga.Release();
                        return default;
                    }
                }
                finally
                {
                    _sagas.Release();
                }
            }

            return await _factory.CreateSagaConsumeContext(_sagas, _context, saga.Instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_sagasLocked)
            {
                _sagas.Release();
                _sagasLocked = false;
            }
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_sagas, consumeContext, instance, mode);
        }
    }


    public class InMemorySagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly IndexedSagaDictionary<TSaga> _sagas;

        public InMemorySagaRepositoryContext(IndexedSagaDictionary<TSaga> sagas, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _sagas = sagas;
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            await _sagas.MarkInUse(CancellationToken).ConfigureAwait(false);
            try
            {
                SagaInstance<TSaga> saga = _sagas[correlationId];
                if (saga == null)
                    return default;

                if (saga.IsRemoved)
                {
                    saga.Release();
                    return default;
                }

                return saga.Instance;
            }
            finally
            {
                _sagas.Release();
            }
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            var matchingInstances = _sagas.Where(query).Select(x => x.Instance.CorrelationId).ToList();

            return new DefaultSagaRepositoryQueryContext<TSaga>(this, matchingInstances);
        }
    }
}
