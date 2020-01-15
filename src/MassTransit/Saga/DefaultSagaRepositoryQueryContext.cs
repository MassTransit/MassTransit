namespace MassTransit.Saga
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class DefaultSagaRepositoryQueryContext<TSaga, T> :
        ConsumeContextProxy<T>,
        SagaRepositoryQueryContext<TSaga, T>
        where TSaga : class, ISaga
        where T : class
    {
        readonly SagaRepositoryContext<TSaga, T> _context;
        readonly IList<Guid> _results;

        public DefaultSagaRepositoryQueryContext(SagaRepositoryContext<TSaga, T> context, IList<Guid> results)
            : base(context)
        {
            _context = context;
            _results = results;
        }

        public int Count => _results.Count;

        public Task<SagaConsumeContext<TSaga, T>> Add(TSaga instance)
        {
            return _context.Add(instance);
        }

        public Task<SagaConsumeContext<TSaga, T>> Insert(TSaga instance)
        {
            return _context.Insert(instance);
        }

        public Task<SagaConsumeContext<TSaga, T>> Load(Guid correlationId)
        {
            return _context.Load(correlationId);
        }

        public Task<SagaRepositoryQueryContext<TSaga, T>> Query(ISagaQuery<TSaga> query)
        {
            return _context.Query(query);
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
