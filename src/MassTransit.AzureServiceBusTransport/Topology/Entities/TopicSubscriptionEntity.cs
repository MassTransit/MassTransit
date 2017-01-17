// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceBus.Messaging;


    public class TopicSubscriptionEntity :
        TopicSubscription,
        TopicSubscriptionHandle
    {
        readonly TopicEntity _destination;
        readonly TopicEntity _source;
        readonly SubscriptionEntity _subscription;

        public TopicSubscriptionEntity(long id, long subscriptionId, TopicEntity source, TopicEntity destination,
            SubscriptionDescription subscriptionDescription)
        {
            Id = id;
            _source = source;
            _destination = destination;
            _subscription = new SubscriptionEntity(subscriptionId, source, subscriptionDescription);
        }

        public static IEqualityComparer<TopicSubscriptionEntity> EntityComparer { get; } = new TopicSubscriptionEntityEqualityComparer();
        public static IEqualityComparer<TopicSubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();

        public Topic Source => _source.Topic;
        public Topic Destination => _destination.Topic;
        public Subscription Subscription => _subscription.Subscription;

        public long Id { get; }
        public TopicSubscription TopicSubscription => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"source: {_source.TopicDescription.Path}",
                $"destination: {_destination.TopicDescription.Path}",
                $"subscription: {_subscription.SubscriptionDescription.Name}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class TopicSubscriptionEntityEqualityComparer :
            IEqualityComparer<TopicSubscriptionEntity>
        {
            public bool Equals(TopicSubscriptionEntity x, TopicSubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return TopicEntity.EntityComparer.Equals(x._source, y._source)
                    && TopicEntity.EntityComparer.Equals(x._destination, y._destination)
                    && SubscriptionEntity.EntityComparer.Equals(x._subscription, y._subscription);
            }

            public int GetHashCode(TopicSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = TopicEntity.EntityComparer.GetHashCode(obj._source);
                    hashCode = (hashCode * 397) ^ TopicEntity.EntityComparer.GetHashCode(obj._destination);
                    hashCode = (hashCode * 397) ^ SubscriptionEntity.EntityComparer.GetHashCode(obj._subscription);

                    return hashCode;
                }
            }
        }

        sealed class NameEqualityComparer :
            IEqualityComparer<TopicSubscriptionEntity>
        {
            public bool Equals(TopicSubscriptionEntity x, TopicSubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.Subscription.SubscriptionDescription.Name, y.Subscription.SubscriptionDescription.Name)
                    && string.Equals(x.Subscription.SubscriptionDescription.TopicPath, y.Subscription.SubscriptionDescription.TopicPath)
                    && string.Equals(x.Destination.TopicDescription.Path, y.Destination.TopicDescription.Path);
            }

            public int GetHashCode(TopicSubscriptionEntity obj)
            {
                var hashCode = obj.Subscription.SubscriptionDescription.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Subscription.SubscriptionDescription.TopicPath.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Destination.TopicDescription.Path.GetHashCode();

                return hashCode;
            }
        }

    }
}