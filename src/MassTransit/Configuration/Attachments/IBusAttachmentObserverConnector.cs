namespace MassTransit.Attachments
{
    using GreenPipes;


    public interface IBusAttachmentObserverConnector
    {
        ConnectHandle ConnectBusAttachmentObserver(IBusAttachmentObserver observer);
    }
}
