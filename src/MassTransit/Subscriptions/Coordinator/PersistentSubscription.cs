// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Subscriptions.Coordinator
{
    using System;

    public class PersistentSubscription
    {
        public PersistentSubscription(Guid peerId, Guid subscriptionId, Uri endpointUri, string messageName,
                                      string correlationId)
        {
            PeerId = peerId;
            SubscriptionId = subscriptionId;
            EndpointUri = endpointUri;
            MessageName = messageName;
            CorrelationId = correlationId;
        }

        public Guid PeerId { get; private set; }
        public Guid SubscriptionId { get; private set; }
        public Uri EndpointUri { get; private set; }
        public string MessageName { get; private set; }
        public string CorrelationId { get; private set; }

        public bool Equals(PersistentSubscription other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.PeerId.Equals(PeerId) && other.SubscriptionId.Equals(SubscriptionId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(PersistentSubscription))
                return false;
            return Equals((PersistentSubscription)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PeerId.GetHashCode()*397) ^ SubscriptionId.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("PeerId: {0}, SubscriptionId: {1}, MessageName: {2}", PeerId, SubscriptionId,
                MessageName);
        }
    }
}