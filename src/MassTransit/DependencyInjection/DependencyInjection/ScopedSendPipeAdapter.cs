namespace MassTransit.DependencyInjection
{
    using System;
    using Transports;


    public class ScopedSendPipeAdapter<TMessage> :
        SendContextPipeAdapter<TMessage>
        where TMessage : class
    {
        readonly IServiceProvider _provider;

        public ScopedSendPipeAdapter(IServiceProvider provider, IPipe<SendContext<TMessage>> pipe)
            : base(pipe)
        {
            _provider = provider;
        }

        protected override void Send<T>(SendContext<T> context)
        {
            context.GetOrAddPayload(() => _provider);
        }

        protected override void Send(SendContext<TMessage> context)
        {
        }
    }
}
