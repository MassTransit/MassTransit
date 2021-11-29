namespace MassTransit.DependencyInjection
{
    using Transports;


    public class ScopedSendPipeAdapter<TScope, TMessage> :
        SendContextPipeAdapter<TMessage>
        where TScope : class
        where TMessage : class
    {
        readonly TScope _payload;

        public ScopedSendPipeAdapter(TScope payload, IPipe<SendContext<TMessage>> pipe)
            : base(pipe)
        {
            _payload = payload;
        }

        protected override void Send<T>(SendContext<T> context)
        {
            context.GetOrAddPayload(() => _payload);
        }

        protected override void Send(SendContext<TMessage> context)
        {
        }
    }
}
