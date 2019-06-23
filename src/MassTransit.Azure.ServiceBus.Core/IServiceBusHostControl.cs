namespace MassTransit.Azure.ServiceBus.Core
{
    using Transports;


    public interface IServiceBusHostControl :
        IServiceBusHost,
        IBusHostControl
    {
    }
}
