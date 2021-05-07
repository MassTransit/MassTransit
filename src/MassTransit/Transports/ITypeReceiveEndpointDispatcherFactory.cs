namespace MassTransit.Transports
{
    public interface ITypeReceiveEndpointDispatcherFactory
    {
        IReceiveEndpointDispatcher Create(IReceiveEndpointDispatcherFactory factory, IEndpointNameFormatter formatter);
    }
}
