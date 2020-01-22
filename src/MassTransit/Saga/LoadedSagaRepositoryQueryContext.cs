namespace MassTransit.Saga
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    /// <summary>
    /// For queries that load the actual saga instances
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class LoadedSagaRepositoryQueryContext<TSaga, TMessage> :
        ConsumeContextProxy<TMessage>,
        SagaRepositoryQueryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly SagaRepositoryContext<TSaga, TMessage> _repositoryContext;
        readonly IList<TSaga> _instances;
        readonly IDictionary<Guid, TSaga> _index;

        public LoadedSagaRepositoryQueryContext(SagaRepositoryContext<TSaga, TMessage> repositoryContext, IList<TSaga> instances)
            : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _instances = instances;

            _index = instances.ToDictionary(x => x.CorrelationId);
        }

        public int Count => _instances.Count;

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _repositoryContext.Add(instance);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            return _repositoryContext.Insert(instance);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            if (_index.TryGetValue(correlationId, out var instance))
            {
                return _repositoryContext.CreateSagaConsumeContext(_repositoryContext, instance, SagaConsumeContextMode.Load);
            }

            return _repositoryContext.Load(correlationId);
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            return _instances.Select(x => x.CorrelationId).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _repositoryContext.CreateSagaConsumeContext(consumeContext, instance, mode);
        }
    }


    /// <summary>
    /// For queries that load the actual saga instances
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class LoadedSagaRepositoryQueryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryQueryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaRepositoryContext<TSaga> _repositoryContext;
        readonly IList<TSaga> _instances;
        readonly IDictionary<Guid, TSaga> _index;

        public LoadedSagaRepositoryQueryContext(SagaRepositoryContext<TSaga> repositoryContext, IList<TSaga> instances)
            : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _instances = instances;

            _index = instances.ToDictionary(x => x.CorrelationId);
        }

        public int Count => _instances.Count;

        public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default)
        {
            return _repositoryContext.Query(query, cancellationToken);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            if (_index.TryGetValue(correlationId, out var instance))
            {
                return Task.FromResult(instance);
            }

            return _repositoryContext.Load(correlationId);
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            return _instances.Select(x => x.CorrelationId).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
