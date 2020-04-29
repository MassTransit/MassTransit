namespace MassTransit.RabbitMqTransport.Integration
{
    public interface IPendingPublish
    {
        void Ack();
        void Nack();
        void PublishNotConfirmed(string reason);
        void PublishReturned(ushort code, string text);
    }
}
