namespace MassTransit.ServiceBus
{
    public interface IEnvelopeFactory
    {
        IEnvelope NewEnvelope(IEndpoint returnTo, params IMessage[] messages);
        IEnvelope NewEnvelope(params IMessage[] messages);
    }
}