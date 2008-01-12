namespace MassTransit.ServiceBus.SubscriptionsManager.Client
{
    using System;
    using log4net;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    /// <summary>
    /// Works with the remote subscription storage to update local subscriptions with the other endpoints.
    /// </summary>
    public class ClientProxy
    {
        private IEndpoint _wellKnownSubscriptionManagerEndpoint;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ClientProxy));
        private IServiceBus _bus;

        public ClientProxy(IEndpoint wellKnownSubscriptionManagerEndpoint)
        {
            _wellKnownSubscriptionManagerEndpoint = wellKnownSubscriptionManagerEndpoint;
        }

        public void StartWatching(IServiceBus bus, ISubscriptionStorage storage)
        {
            _bus = bus;
            _bus.Subscribe<CacheUpdateResponse>(null);
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, new RequestCacheUpdate());

            storage.SubscriptionChanged += storage_SubscriptionChanged;

            foreach (Uri uri in storage.List())
            {
                SendUpdate(new SubscriptionChange("", uri, SubscriptionChange.SubscriptionChangeType.Add));
            }
        }

        private void storage_SubscriptionChanged(object sender, SubscriptionChangedEventArgs e)
        {
            //send stuff to the wellknown endpoint
            SendUpdate(e.Change);
            
        }

        public void SendUpdate(SubscriptionChange change)
        {
            _bus.Send(_wellKnownSubscriptionManagerEndpoint, change);
        }
    }
}
