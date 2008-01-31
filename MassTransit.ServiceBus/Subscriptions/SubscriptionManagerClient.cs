namespace MassTransit.ServiceBus.Subscriptions
{
    using System;
    using Messages;

    public class SubscriptionManagerClient :
        IDisposable
    {
        private readonly IServiceBus _serviceBus;
        private readonly IMessageQueueEndpoint _managerEndpoint;

        public SubscriptionManagerClient(IServiceBus serviceBus, IMessageQueueEndpoint managerEndpoint)
        {
            _serviceBus = serviceBus;
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
            // TODO update the local cache with the response
            
        }
    }
}