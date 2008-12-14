namespace MassTransit.Metadata.Server
{
    using Messages;

    public class MessageConsumer :
        Consumes<Metadata>.All
    {
        public void Consume(Metadata message)
        {
            
        }
    }
}