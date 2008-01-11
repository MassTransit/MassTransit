using System;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus.Subscriptions
{
    public class SubscriptionCacheEntry :
        IEquatable<SubscriptionCacheEntry>
    {
        private Uri _endpoint;

    	private MessageId _messageId = string.Empty;

        public SubscriptionCacheEntry(Uri endpoint)
        {
            _endpoint = endpoint;
        }

        public Uri Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

		public MessageId MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        #region IEquatable<SubscriptionCacheEntry> Members

        public bool Equals(SubscriptionCacheEntry other)
        {
            if (other == null)
                return false;

			if (_endpoint != other.Endpoint)
				return false;

			if (_messageId != other.MessageId)
				return false;

        	return true;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as SubscriptionCacheEntry);
        }

        public override int GetHashCode()
        {
            return _endpoint.GetHashCode() + 29*_messageId.GetHashCode();
        }

    }
}