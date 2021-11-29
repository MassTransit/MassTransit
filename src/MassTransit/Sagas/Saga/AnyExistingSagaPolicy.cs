namespace MassTransit.Saga
{
    using System.Threading.Tasks;


    /// <summary>
    /// Sends the message to any existing saga instances, failing silently if no saga instances are found.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class AnyExistingSagaPolicy<TSaga, TMessage> :
        ISagaPolicy<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IPipe<ConsumeContext<TMessage>> _missingPipe;

        public AnyExistingSagaPolicy(IPipe<ConsumeContext<TMessage>> missingPipe = null, bool readOnly = false)
        {
            IsReadOnly = readOnly;
            _missingPipe = missingPipe ?? Pipe.Empty<ConsumeContext<TMessage>>();
        }

        public bool IsReadOnly { get; }

        public bool PreInsertInstance(ConsumeContext<TMessage> context, out TSaga instance)
        {
            instance = null;
            return false;
        }

        Task ISagaPolicy<TSaga, TMessage>.Existing(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            return next.Send(context);
        }

        Task ISagaPolicy<TSaga, TMessage>.Missing(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            return _missingPipe.Send(context);
        }
    }
}
