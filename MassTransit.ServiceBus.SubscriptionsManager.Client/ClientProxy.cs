namespace MassTransit.ServiceBus.SubscriptionsManager.Client
{
    using System;
    using log4net;
    using MassTransit.ServiceBus.Subscriptions.Messages;

    public class ClientProxy
    {
        //works with the ISubscriptionStorage to send messages back and forth.

        private IEndpoint _wellKnownSubscriptionManagerEndpoint;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ClientProxy));

        public ClientProxy(IEndpoint wellKnownSubscriptionManagerEndpoint)
        {
            _wellKnownSubscriptionManagerEndpoint = wellKnownSubscriptionManagerEndpoint;
        }

        public void StartWatching(ISubscriptionStorage storage)
        {
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
            
        }
    }
}
