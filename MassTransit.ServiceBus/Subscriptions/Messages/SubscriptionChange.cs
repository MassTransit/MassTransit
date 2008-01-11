using System;

namespace MassTransit.ServiceBus.Subscriptions.Messages
{
    [Serializable]
    public class SubscriptionChange : IEquatable<SubscriptionChange>, IMessage
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

        public SubscriptionChange(Type messageType, Uri address, SubscriptionChangeType changeType) : this(messageType.FullName, address, changeType)
        {
        }

        public SubscriptionChange(string messageName, Uri address, SubscriptionChangeType changeType)
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

        public bool Equals(SubscriptionChange subscriptionChange)
        {
            if (subscriptionChange == null) return false;
            return
                Equals(_messageName, subscriptionChange._messageName) && Equals(_address, subscriptionChange._address);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as SubscriptionChange);
        }

        public override int GetHashCode()
        {
            return _messageName.GetHashCode() + 29*_address.GetHashCode();
        }
    }
}