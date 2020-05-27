namespace MassTransit.Attachments
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Util;


    public class BusAttachmentObservable :
        Connectable<IBusAttachmentObserver>,
        IBusAttachmentObserver
    {
        public Task ConnectFaulted(Exception exception)
        {
            return ForEachAsync(x => x.ConnectFaulted(exception));
        }

        public Task PreConnect(IBusAttachment busAttachment)
        {
            return ForEachAsync(x => x.PreConnect(busAttachment));
        }

        public Task PostConnect(IBusAttachment busAttachment)
        {
            return ForEachAsync(x => x.PostConnect(busAttachment));
        }

        public Task DisconnectFaulted(Exception exception)
        {
            return ForEachAsync(x => x.DisconnectFaulted(exception));
        }

        public Task PreDisconnect(IBusAttachment busAttachment)
        {
            return ForEachAsync(x => x.PreDisconnect(busAttachment));
        }

        public Task PostDisconnect(IBusAttachment busAttachment)
        {
            return ForEachAsync(x => x.PostDisconnect(busAttachment));
        }
    }
}
