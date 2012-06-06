namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit;
    using MassTransit.Pipeline;

    /// <summary>
    /// Decorates a saga repository with a callback method that is invoked before every
    /// instance of the saga is returned, allowing any dependencies to be set.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class DelegatingSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly Action<TSaga> _callback;
        readonly ISagaRepository<TSaga> _repository;

        public DelegatingSagaRepository(ISagaRepository<TSaga> repository, Action<TSaga> callback)
        {
            _repository = repository;
            _callback = callback;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context, Guid sagaId,
                                                                                InstanceHandlerSelector<TSaga, TMessage> selector,
                                                                                ISagaPolicy<TSaga, TMessage> policy)
            where TMessage : class
        {
            return _repository.GetSaga(context, sagaId, (saga, message) =>
                {
                    _callback(saga);

                    return selector(saga, message);
                }, policy);
        }

        public IEnumerable<Guid> Find(ISagaFilter<TSaga> filter)
        {
            return _repository.Find(filter);
        }

        public IEnumerable<TSaga> Where(ISagaFilter<TSaga> filter)
        {
            return _repository.Where(filter).Select(x =>
                {
                    _callback(x);
                    return x;
                });
        }

        public IEnumerable<TResult> Where<TResult>(ISagaFilter<TSaga> filter, Func<TSaga, TResult> transformer)
        {
            return _repository.Where(filter, x =>
                {
                    _callback(x);
                    return transformer(x);
                });
        }

        public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
        {
            return _repository.Select(x =>
                {
                    _callback(x);
                    return transformer(x);
                });
        }
    }
}