namespace MassTransit.Metadata.Server
{
    using Messages;

    public class EndpointConsumer :
        Consumes<EndpointModel>.All
    {
        public void Consume(EndpointModel message)
        {
            
        }
    }
}