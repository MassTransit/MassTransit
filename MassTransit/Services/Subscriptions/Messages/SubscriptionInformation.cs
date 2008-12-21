namespace MassTransit.Services.Subscriptions.Messages
{
    using System;
    using MassTransit.Subscriptions;

    [Serializable]
    public class SubscriptionInformation :
        IEquatable<SubscriptionInformation>
    {
        private string _correlationId;
        protected Uri _endpointUri;
        private string _messageName;

        protected SubscriptionInformation()
        {
        }

        public SubscriptionInformation(string messageName, Uri endpointUri)
        {
            _endpointUri = endpointUri;
            _messageName = messageName.Trim();
            _correlationId = String.Empty;
        }

        public SubscriptionInformation(Type messageType, Uri endpointUri)
        {
            _endpointUri = endpointUri;
            _messageName = BuildMessageName(messageType);
            _correlationId = String.Empty;
        }

        public SubscriptionInformation(string messageName, string correlationId, Uri endpointUri)
        {
            _endpointUri = endpointUri;
            _messageName = messageName.Trim();
            _correlationId = correlationId;
        }

        public SubscriptionInformation(Type messageType, string correlationId, Uri endpointUri)
        {
            _endpointUri = endpointUri;
            _messageName = BuildMessageName(messageType);
            _correlationId = correlationId;
        }

        public SubscriptionInformation(Subscription subscription)
        {
            _endpointUri = subscription.EndpointUri;
            _messageName = subscription.MessageName.Trim();
            _correlationId = subscription.CorrelationId;
        }

        public Uri EndpointUri
        {
            get { return _endpointUri; }
        }

        public string MessageName
        {
            get { return _messageName; }
        }

        public string CorrelationId
        {
            get { return _correlationId; }
        }

        public bool Equals(SubscriptionInformation subscription)
        {
            if (subscription == null) return false;
            if (!_endpointUri.Equals(subscription._endpointUri)) return false;
            if (!String.Equals(_messageName, subscription._messageName)) return false;
            if (!String.Equals(_correlationId, subscription._correlationId)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) 
                return true;

            return Equals(obj as SubscriptionInformation);
        }

        public override int GetHashCode()
        {
            int result = _endpointUri.GetHashCode();
            result = 29*result + _messageName.GetHashCode();
            result = 29*result + _correlationId.GetHashCode();
            return result;
        }

        public static string BuildMessageName(Type t)
        {
            return t.FullName;
        }
    }
}