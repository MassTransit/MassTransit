namespace MassTransit.Services.Metadata.Server
{
    using Messages;

    public class MessageConsumer :
        Consumes<MessageDefinition>.All
    {
        public void Consume(MessageDefinition message)
        {
            
        }
    }
}