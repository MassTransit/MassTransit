namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using Messages;

    public class SubscriptionManagerClient :
        IDisposable
    {
        private readonly IServiceBus _serviceBus;
        private readonly ISubscriptionStorage _cache;
        private readonly IMessageQueueEndpoint _managerEndpoint;

        public SubscriptionManagerClient(IServiceBus serviceBus, ISubscriptionStorage cache, IMessageQueueEndpoint managerEndpoint)
        {
            _serviceBus = serviceBus;
            _cache = cache;
            _managerEndpoint = managerEndpoint;
        }

        public void Dispose()
        {
            
        }

        public void Start()
        {
            IServiceBusAsyncResult asyncResult = _serviceBus.Request<CacheUpdateRequest>(_managerEndpoint, CacheUpdateResponse_Callback, this, new CacheUpdateRequest()); 
        }

        public void CacheUpdateResponse_Callback(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                return;

            IServiceBusAsyncResult serviceBusAsyncResult = asyncResult as IServiceBusAsyncResult;
            if (serviceBusAsyncResult == null)
                return;

            if (serviceBusAsyncResult.Messages == null)
                return;

            foreach(IMessage message in serviceBusAsyncResult.Messages)
            {
                CacheUpdateResponse response = message as CacheUpdateResponse;
                if(response != null)
                {
                    foreach(Subscription sub in response.Subscriptions)
                    {
                        _cache.Add(sub.MessageName, sub.Address);
                    }
                }
            }
        }
    }
}