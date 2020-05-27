namespace MassTransit.Attachments
{
    using System;
    using System.Threading.Tasks;


    public interface IBusAttachmentObserver
    {
        Task ConnectFaulted(Exception exception);
        Task PreConnect(IBusAttachment busAttachment);
        Task PostConnect(IBusAttachment busAttachment);

        Task DisconnectFaulted(Exception exception);
        Task PreDisconnect(IBusAttachment busAttachment);
        Task PostDisconnect(IBusAttachment busAttachment);
    }
}
