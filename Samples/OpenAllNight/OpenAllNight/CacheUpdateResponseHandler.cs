namespace OpenAllNight
{
    using System;
    using log4net;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    public class CacheUpdateResponseHandler : 
        Consumes<CacheUpdateResponse>.All
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (CacheUpdateResponseHandler));


        public void Consume(CacheUpdateResponse message)
        {
            _log.InfoFormat("Received update message number");
        }
    }
}