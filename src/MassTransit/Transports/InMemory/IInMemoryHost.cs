namespace MassTransit.Transports.InMemory
{
    public interface IInMemoryHost :
        IHost,
        IReceiveConnector<IInMemoryReceiveEndpointConfigurator>
    {
    }
}
