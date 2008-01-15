using System;

namespace MassTransit.ServiceBus.Subscriptions.Messages
{
    [Serializable]
    public class SubscriptionChange : IMessage
    {
        #region SubscriptionChangeType enum

        #endregion

        private Subscription _subscription;
        private SubscriptionChangeType _changeType;


        public SubscriptionChange(Type messageType, Uri address, SubscriptionChangeType changeType) : this(messageType.FullName, address, changeType)
        {
        }

        public SubscriptionChange(string messageName, Uri address, SubscriptionChangeType changeType): this(new Subscription(address, messageName), changeType)
        {
        }

        public SubscriptionChange(Subscription subscription, SubscriptionChangeType changeType)
        {
            _changeType = changeType;
            _subscription = subscription;
        }

        public SubscriptionChangeType ChangeType
        {
            get { return _changeType; }
        }


        public Subscription Subscription
        {
            get { return _subscription; }
        }

    }
}