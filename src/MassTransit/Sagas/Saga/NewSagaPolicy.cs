namespace MassTransit.Saga
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Accepts a message to a saga that does not already exist, throwing an exception if an existing
    /// saga instance is specified.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class NewSagaPolicy<TSaga, TMessage> :
        ISagaPolicy<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly bool _insertOnInitial;
        readonly ISagaFactory<TSaga, TMessage> _sagaFactory;

        public NewSagaPolicy(ISagaFactory<TSaga, TMessage> sagaFactory, bool insertOnInitial)
        {
            _sagaFactory = sagaFactory;
            _insertOnInitial = insertOnInitial;
        }

        public bool IsReadOnly => false;

        public bool PreInsertInstance(ConsumeContext<TMessage> context, out TSaga instance)
        {
            if (_insertOnInitial)
            {
                instance = _sagaFactory.Create(context);
                return true;
            }

            instance = null;
            return false;
        }

        Task ISagaPolicy<TSaga, TMessage>.Existing(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            throw new SagaException("The message cannot be accepted by an existing saga", typeof(TSaga), typeof(TMessage),
                context.CorrelationId ?? Guid.Empty);
        }

        Task ISagaPolicy<TSaga, TMessage>.Missing(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            return _sagaFactory.Send(context, next);
        }
    }
}
