namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        readonly ISagaRepository<TSaga> _repository;
        readonly Action<SagaConsumeContext<TSaga>> _callback;

        public DelegatingSagaRepository(ISagaRepository<TSaga> repository, Action<SagaConsumeContext<TSaga>> callback)
        {
            _repository = repository;
            _callback = callback;
        }

//        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context, Guid sagaId,
//                                                                                InstanceHandlerSelector<TSaga, TMessage> selector,
//                                                                                ISagaPolicy<TSaga, TMessage> policy)
//            where TMessage : class
//        {
//            return _repository.GetSaga(context, sagaId, (saga, message) =>
//                {
//                    _callback(saga);
//
//                    return selector(saga, message);
//                }, policy);
//        }

        Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            var callbackPipe = Pipe.New<SagaConsumeContext<TSaga, T>>(x =>
            {
                x.Execute(_callback);
                x.ExecuteAsync(next.Send);
            });

            return _repository.Send(context, callbackPipe);
        }

        public IEnumerable<Guid> Find(ISagaFilter<TSaga> filter)
        {
            return _repository.Find(filter);
        }

        public IEnumerable<TSaga> Where(ISagaFilter<TSaga> filter)
        {
            return _repository.Where(filter).Select(x =>
                {
                    return x;
                });
        }

        public IEnumerable<TResult> Where<TResult>(ISagaFilter<TSaga> filter, Func<TSaga, TResult> transformer)
        {
            return _repository.Where(filter, x =>
                {
                    return transformer(x);
                });
        }

        public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
        {
            return _repository.Select(x =>
                {
                    return transformer(x);
                });
        }
    }
}