// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

    public class PersistentSubscription : 
        IEquatable<PersistentSubscription>
    {
        public PersistentSubscription(Uri busUri, Guid peerId, Guid subscriptionId, Uri endpointUri, string messageName,
            string correlationId)
        {
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
            BusUri = busUri;
            PeerId = peerId;
            SubscriptionId = subscriptionId;
            EndpointUri = endpointUri;
            MessageName = messageName;
            CorrelationId = correlationId;
        }

        protected PersistentSubscription()
        {
        }

        public Uri BusUri { get; private set; }
        public Guid PeerId { get; private set; }
        public Guid SubscriptionId { get; private set; }
        public Uri EndpointUri { get; private set; }
        public string MessageName { get; private set; }
        public string CorrelationId { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Updated { get; set; }

        public bool Equals(PersistentSubscription other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.BusUri, BusUri) && other.PeerId.Equals(PeerId) && other.SubscriptionId.Equals(SubscriptionId);
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
                int result = (BusUri != null
                                  ? BusUri.GetHashCode()
                                  : 0);
                result = (result*397) ^ PeerId.GetHashCode();
                result = (result*397) ^ SubscriptionId.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(PersistentSubscription left, PersistentSubscription right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PersistentSubscription left, PersistentSubscription right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("PeerId: {0}, SubscriptionId: {1}, MessageName: {2}", PeerId, SubscriptionId,
                MessageName);
        }
    }
}