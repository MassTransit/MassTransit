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

    public class PersistentSubscription : IEquatable<PersistentSubscription>
    {
        public PersistentSubscription(Uri busUri, Guid peerId, Uri peerUri, Guid subscriptionId, Uri endpointUri,
            string messageName, string correlationId)
        {
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
            BusUri = busUri;
            PeerUri = peerUri;
            PeerId = peerId;
            SubscriptionId = subscriptionId;
            EndpointUri = endpointUri;
            MessageName = messageName;
            CorrelationId = correlationId;
        }

        protected PersistentSubscription()
        {
        }

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

        /// <summary>
        /// The control bus address of the instance this subscription belongs to
        /// </summary>
        public Uri BusUri { get; private set; }

        /// <summary>
        /// The PeerId of the remote subscription
        /// </summary>
        public Guid PeerId { get; private set; }

        /// <summary>
        /// The control bus address of the peer
        /// </summary>
        public Uri PeerUri { get; private set; }

        /// <summary>
        /// The subscription id for this instance of the subscription
        /// </summary>
        public Guid SubscriptionId { get; private set; }

        /// <summary>
        /// The endpointUri where messages should be delivered matching the subscription
        /// </summary>
        public Uri EndpointUri { get; private set; }

        /// <summary>
        /// The message name of the subscription
        /// </summary>
        public string MessageName { get; private set; }

        /// <summary>
        /// The correlationId of the subscription if it is correlated
        /// </summary>
        public string CorrelationId { get; private set; }

        /// <summary>
        /// The time the subscription was created
        /// </summary>
        public DateTime Created { get; private set; }

        /// <summary>
        /// The time the subscription was last updated
        /// </summary>
        public DateTime Updated { get; set; }


        public override string ToString()
        {
            return string.Format("PeerId: {0}, PeerUri: {1} SubId: {2} Type: {3}", PeerId, PeerUri, SubscriptionId, MessageName);
        }
    }
}