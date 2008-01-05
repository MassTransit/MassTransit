namespace MassTransit.ServiceBus
{
    public interface IEnvelopeConsumer
    {
        bool MeetsCriteria(IEnvelope envelope);

        void Deliver(IEnvelope envelope);
    }
}