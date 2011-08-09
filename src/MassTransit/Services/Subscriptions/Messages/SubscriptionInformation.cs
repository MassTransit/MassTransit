// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Services.Subscriptions.Messages
{
	using System;
	using Magnum;

	[Serializable]
	public class SubscriptionInformation :
		IEquatable<SubscriptionInformation>
	{
		public SubscriptionInformation(Guid clientId, long sequenceNumber, Type messageType, Uri endpointUri)
			: this(clientId, sequenceNumber, messageType.ToMessageName(), "", endpointUri)
		{
		}

		public SubscriptionInformation(Guid clientId, long sequenceNumber, Type messageType, string correlationId, Uri endpointUri)
			: this(clientId, sequenceNumber, messageType.ToMessageName(), correlationId, endpointUri)
		{
		}

		public SubscriptionInformation(Guid clientId, long sequenceNumber, string messageName, string correlationId, Uri endpointUri)
		{
			ClientId = clientId;
			SequenceNumber = sequenceNumber;

			MessageName = messageName.Trim();
			CorrelationId = correlationId;
			EndpointUri = endpointUri;

			SubscriptionId = CombGuid.Generate();
		}

		public SubscriptionInformation(Guid clientId, Guid subscriptionId, string messageName, string correlationId, Uri endpointUri)
		{
			ClientId = clientId;
			SequenceNumber = 0;

			MessageName = messageName.Trim();
			CorrelationId = correlationId;
			EndpointUri = endpointUri;

			SubscriptionId = subscriptionId;
		}

		protected SubscriptionInformation()
		{
		}

		public Guid ClientId { get; set; }
		public long SequenceNumber { get; set; }

		public string MessageName { get; set; }
		public string CorrelationId { get; set; }
		public Uri EndpointUri { get; set; }

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
			return MessageName + " " + (CorrelationId ?? "-") + " " + EndpointUri + " [" + SubscriptionId + "]";
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