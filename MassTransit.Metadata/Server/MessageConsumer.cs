namespace MassTransit.Metadata.Server
{
    using Messages;

    public class MessageConsumer :
        Consumes<MetadataMessage>.All
    {
        public void Consume(MetadataMessage message)
        {
            
        }
    }
}