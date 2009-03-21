namespace MassTransit.Services.Subscriptions.Messages
{
	using System;
	using Magnum;
	using MassTransit.Subscriptions;

	[Serializable]
	public class SubscriptionInformation :
		IEquatable<SubscriptionInformation>
	{
		protected SubscriptionInformation()
		{
		}

		public SubscriptionInformation(Type messageType, Uri endpointUri)
			: this(messageType.ToMessageName(), string.Empty, endpointUri)
		{
		}

		public SubscriptionInformation(string messageName, Uri endpointUri)
			: this(messageName, string.Empty, endpointUri)
		{
		}

		public SubscriptionInformation(Type messageType, string correlationId, Uri endpointUri)
			: this(messageType.ToMessageName(), correlationId, endpointUri)
		{
		}

		public SubscriptionInformation(string messageName, string correlationId, Uri endpointUri)
		{
			EndpointUri = endpointUri;
			MessageName = messageName.Trim();
			CorrelationId = correlationId;

			SubscriptionId = CombGuid.Generate();
		}

		public SubscriptionInformation(Subscription subscription)
		{
			EndpointUri = subscription.EndpointUri;
			MessageName = subscription.MessageName.Trim();
			CorrelationId = subscription.CorrelationId;
		}

		public Uri EndpointUri { get; set; }

		public string MessageName { get; set; }

		public string CorrelationId { get; set; }

		public Guid SubscriptionId { get; set; }

		public bool Equals(SubscriptionInformation subscription)
		{
			if (subscription == null) return false;
			if (!EndpointUri.Equals(subscription.EndpointUri)) return false;
			if (!string.Equals(MessageName, subscription.MessageName)) return false;
			if (!string.Equals(CorrelationId, subscription.CorrelationId)) return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			return Equals(obj as SubscriptionInformation);
		}

		public override string ToString()
		{
			return MessageName + " " + (CorrelationId ?? "-" ) + " " + EndpointUri + " [" + SubscriptionId + "]";
		}

		public override int GetHashCode()
		{
			int result = EndpointUri.GetHashCode();
			result = 29*result + MessageName.GetHashCode();
			result = 29*result + CorrelationId.GetHashCode();
			return result;
		}
	}
}