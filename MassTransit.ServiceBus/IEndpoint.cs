namespace MassTransit.ServiceBus
{
    public interface IEndpoint
    {
        ITransport Transport { get; }
    }
}