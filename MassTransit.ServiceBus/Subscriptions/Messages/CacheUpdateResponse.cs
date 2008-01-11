namespace MassTransit.ServiceBus.Subscriptions.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CacheUpdateResponse : IMessage
    {
        private List<SubscriptionChange> _subscriptions;


        public CacheUpdateResponse(List<SubscriptionChange> subscriptions)
        {
            _subscriptions = subscriptions;
        }


        public List<SubscriptionChange> Subscriptions
        {
            get { return _subscriptions; }
        }
    }
}