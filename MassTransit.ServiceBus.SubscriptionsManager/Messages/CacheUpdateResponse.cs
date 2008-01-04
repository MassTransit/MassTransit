namespace MassTransit.ServiceBus.SubscriptionsManager.Messages
{
    using System;
    using System.Collections.Generic;
    using Subscriptions;

    [Serializable]
    public class CacheUpdateResponse : IMessage
    {
        private List<SubscriptionMessage> _subscriptions;


        public CacheUpdateResponse(List<SubscriptionMessage> subscriptions)
        {
            _subscriptions = subscriptions;
        }


        public List<SubscriptionMessage> Subscriptions
        {
            get { return _subscriptions; }
        }
    }
}