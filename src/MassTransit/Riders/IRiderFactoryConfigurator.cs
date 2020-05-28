namespace MassTransit.Riders
{
    using Transports;


    public interface IRiderFactoryConfigurator :
        IRiderObserverConnector,
        IReceiveEndpointObserverConnector
    {
    }
}
