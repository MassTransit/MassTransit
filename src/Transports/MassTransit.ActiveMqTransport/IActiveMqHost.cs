namespace MassTransit.ActiveMqTransport
{
    using Transports;


    public interface IActiveMqHost :
        IHost<IActiveMqReceiveEndpointConfigurator>
    {
    }
}
