namespace MassTransit.LegacySupport.ProxyMessages
{
    using System;

    [Serializable]
    public abstract class SubscriptionChange
    {
        Subscription _subscription;

        //xml serialization
        protected SubscriptionChange()
        {
        }

        public SubscriptionChange(string messageName, Uri address)
            : this(new Subscription(messageName, address))
        {
        }

        public SubscriptionChange(Subscription subscription)
        {
            _subscription = subscription;
        }

        public Subscription Subscription
        {
            get { return _subscription; }
            set { _subscription = value; }
        }
    }
}