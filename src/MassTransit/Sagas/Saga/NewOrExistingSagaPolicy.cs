namespace MassTransit.Saga
{
    using System.Threading.Tasks;


    /// <summary>
    /// Creates a new or uses an existing saga instance
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class NewOrExistingSagaPolicy<TSaga, TMessage> :
        ISagaPolicy<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly bool _insertOnInitial;
        readonly ISagaFactory<TSaga, TMessage> _sagaFactory;

        public NewOrExistingSagaPolicy(ISagaFactory<TSaga, TMessage> sagaFactory, bool insertOnInitial)
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
            return next.Send(context);
        }

        Task ISagaPolicy<TSaga, TMessage>.Missing(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            return _sagaFactory.Send(context, next);
        }
    }
}
