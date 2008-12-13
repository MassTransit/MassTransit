namespace MassTransit.Metadata.Server
{
    using Messages;

    public class TransmissionConsumer :
        Consumes<TransmissionModel>.All
    {
        public void Consume(TransmissionModel message)
        {
            
        }
    }
}