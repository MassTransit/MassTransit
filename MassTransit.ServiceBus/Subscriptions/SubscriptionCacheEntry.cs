using System;

namespace MassTransit.ServiceBus.Subscriptions
{
    public class SubscriptionCacheEntry :
        IEquatable<SubscriptionCacheEntry>
    {
        private IEndpoint _endpoint;

    	private string _messageId = string.Empty;

        public SubscriptionCacheEntry(IEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public IEndpoint Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        public string MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        #region IEquatable<SubscriptionCacheEntry> Members

        public bool Equals(SubscriptionCacheEntry other)
        {
            if (other == null)
                return false;

			if (string.Compare(_endpoint.Address, other.Endpoint.Address, true) != 0)
				return false;

			if (string.Compare(_messageId, other.MessageId, true) != 0)
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