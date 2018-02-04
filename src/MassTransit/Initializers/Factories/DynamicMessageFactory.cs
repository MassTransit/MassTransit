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
}
