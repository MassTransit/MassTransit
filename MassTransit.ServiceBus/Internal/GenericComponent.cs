namespace MassTransit.ServiceBus.Internal
{
    using System;

    public class GenericComponent<TMessage> :
        Consumes<TMessage>.Any where TMessage : class, IMessage
    {
        private Action<IMessageContext<TMessage>> _wrappedAction;
        private IServiceBus _bus;

        public GenericComponent(Action<IMessageContext<TMessage>> wrappedAction, IServiceBus bus)
        {
            _wrappedAction = wrappedAction;
            _bus = bus;
        }

        public void Consume(TMessage message)
        {
            IEnvelope notSureHowToGet = null;
            IMessageContext<TMessage> cxt = new MessageContext<TMessage>(_bus, notSureHowToGet, message);
            _wrappedAction(cxt);
        }
    }
}