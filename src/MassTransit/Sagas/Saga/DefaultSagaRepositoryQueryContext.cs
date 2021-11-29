namespace MassTransit.Saga
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Middleware;


    public class DefaultSagaRepositoryQueryContext<TSaga, TMessage> :
        ConsumeContextProxy<TMessage>,
        SagaRepositoryQueryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly SagaRepositoryContext<TSaga, TMessage> _context;
        readonly IList<Guid> _results;

        public DefaultSagaRepositoryQueryContext(SagaRepositoryContext<TSaga, TMessage> context, IList<Guid> results)
            : base(context)
        {
            _context = context;
            _results = results;
        }

        public int Count => _results.Count;

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _context.Add(instance);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            return _context.Insert(instance);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            return _context.Load(correlationId);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return _context.Save(context);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return _context.Discard(context);
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return _context.Undo(context);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            return _context.Update(context);
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _context.Delete(context);
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _context.CreateSagaConsumeContext(consumeContext, instance, mode);
        }
    }


    public class DefaultSagaRepositoryQueryContext<TSaga> :
        ProxyPipeContext,
        SagaRepositoryQueryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaRepositoryContext<TSaga> _context;
        readonly IList<Guid> _results;

        public DefaultSagaRepositoryQueryContext(SagaRepositoryContext<TSaga> context, IList<Guid> results)
            : base(context)
        {
            _context = context;
            _results = results;
        }

        public int Count => _results.Count;

        public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            return _context.Query(query, cancellationToken);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.Load(correlationId);
        }

        public IEnumerator<Guid> GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
