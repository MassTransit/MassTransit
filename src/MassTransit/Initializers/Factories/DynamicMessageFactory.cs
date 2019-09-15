namespace MassTransit.Initializers.Factories
{
    public class DynamicMessageFactory<TMessage, TImplementation> :
        IMessageFactory<TMessage>
        where TMessage : class
        where TImplementation : TMessage, new()
    {
        public InitializeContext<TMessage> Create(InitializeContext context)
        {
            var message = new TImplementation();

            return context.CreateMessageContext<TMessage>(message);
        }
    }


    public class DynamicMessageFactory<TMessage> :
        IMessageFactory<TMessage>,
        IMessageFactory
        where TMessage : class, new()
    {
        public InitializeContext<TMessage> Create(InitializeContext context)
        {
            var message = new TMessage();

            return context.CreateMessageContext(message);
        }

        public object Create()
        {
            return new TMessage();
        }
    }
}
