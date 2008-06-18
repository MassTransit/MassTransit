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
        private readonly IServiceBus _bus;
        private readonly IEndpoint _subscriptionServiceEndpoint;
        private readonly Int64 _targetCount;

        private static Int64 _counter = 0;

        public CacheUpdateResponseHandler(IServiceBus bus, IEndpoint subscriptionServiceEndpoint, Int64 targetCount)
        {
            _bus = bus;
            _subscriptionServiceEndpoint = subscriptionServiceEndpoint;
            _targetCount = targetCount;
        }


        public void Consume(CacheUpdateResponse message)
        {
            _log.InfoFormat("Received update message number {0}", ++_counter);
            if(_counter < _targetCount)
                _subscriptionServiceEndpoint.Send(new CacheUpdateRequest(_bus.Endpoint.Uri));
        }


        public static long Counter
        {
            get { return _counter; }
        }
    }
}