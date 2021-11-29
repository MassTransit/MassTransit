namespace MassTransit.Saga
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Middleware;


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
        readonly IDictionary<Guid, TSaga> _index;
        readonly SagaRepositoryContext<TSaga, TMessage> _repositoryContext;

        public LoadedSagaRepositoryQueryContext(SagaRepositoryContext<TSaga, TMessage> repositoryContext, IEnumerable<TSaga> instances)
            : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;

            _index = instances.ToDictionary(x => x.CorrelationId);
        }

        public int Count => _index.Count;

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
                return _repositoryContext.CreateSagaConsumeContext(_repositoryContext, instance, SagaConsumeContextMode.Load);

            return _repositoryContext.Load(correlationId);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return _repositoryContext.Save(context);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return _repositoryContext.Discard(context);
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return _repositoryContext.Undo(context);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            return _repositoryContext.Update(context);
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _repositoryContext.Delete(context);
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            return _index.Keys.GetEnumerator();
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
        readonly IDictionary<Guid, TSaga> _index;
        readonly SagaRepositoryContext<TSaga> _repositoryContext;

        public LoadedSagaRepositoryQueryContext(SagaRepositoryContext<TSaga> repositoryContext, IEnumerable<TSaga> instances)
            : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;

            _index = instances.ToDictionary(x => x.CorrelationId);
        }

        public int Count => _index.Count;

        public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default)
        {
            return _repositoryContext.Query(query, cancellationToken);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            if (_index.TryGetValue(correlationId, out var instance))
                return Task.FromResult(instance);

            return _repositoryContext.Load(correlationId);
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            return _index.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
