namespace MassTransit.Transports.InMemory
{
    public interface IInMemoryHost :
        IHost<IInMemoryReceiveEndpointConfigurator>
    {
    }
}
