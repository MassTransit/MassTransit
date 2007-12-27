using System;

namespace MassTransit.ServiceBus.Subscriptions
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

        private string _address;

        private SubscriptionChangeType _changeType;
        private Type _messageType;

        public SubscriptionMessage(Type messageType, string address, SubscriptionChangeType changeType)
        {
            _messageType = messageType;
            _address = address;
            _changeType = changeType;
        }

        public SubscriptionChangeType ChangeType
        {
            get { return _changeType; }
            set { _changeType = value; }
        }


        public Type MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        #region IEquatable<SubscriptionMessage> Members

        public bool Equals(SubscriptionMessage subscriptionMessage)
        {
            if (subscriptionMessage == null) return false;
            return
                Equals(_messageType, subscriptionMessage._messageType) && Equals(_address, subscriptionMessage._address);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as SubscriptionMessage);
        }

        public override int GetHashCode()
        {
            return _messageType.GetHashCode() + 29*_address.GetHashCode();
        }
    }
}