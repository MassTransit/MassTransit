namespace MassTransit.Attachments
{
    using Transports;


    public interface IBusAttachmentFactoryConfigurator :
        IBusAttachmentObserverConnector,
        IReceiveEndpointObserverConnector
    {
        string Name { get; }
    }
}
