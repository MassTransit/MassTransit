using System;

namespace MassTransit.ServiceBus.Subscriptions.Messages
{
    [Serializable]
    public class SubscriptionMessage : IEquatable<SubscriptionMessage>, IMessage
    {
        #region SubscriptionChangeType enum

        public enum SubscriptionChangeType
        {
            Add,

            Remove,
        }

        #endregion

        private Uri _address;

        private SubscriptionChangeType _changeType;
        private string _messageName;

        public SubscriptionMessage(Type messageType, Uri address, SubscriptionChangeType changeType) : this(messageType.FullName, address, changeType)
        {
        }

        public SubscriptionMessage(string messageName, Uri address, SubscriptionChangeType changeType)
        {
            _messageName = messageName;
            _address = address;
            _changeType = changeType;
        }

        public SubscriptionChangeType ChangeType
        {
            get { return _changeType; }
            set { _changeType = value; }
        }


        public string MessageName
        {
            get { return _messageName; }
            set { _messageName = value; }
        }

        public Uri Address
        {
            get { return _address; }
            set { _address = value; }
        }

        #region IEquatable<SubscriptionMessage> Members

        public bool Equals(SubscriptionMessage subscriptionMessage)
        {
            if (subscriptionMessage == null) return false;
            return
                Equals(_messageName, subscriptionMessage._messageName) && Equals(_address, subscriptionMessage._address);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as SubscriptionMessage);
        }

        public override int GetHashCode()
        {
            return _messageName.GetHashCode() + 29*_address.GetHashCode();
        }
    }
}