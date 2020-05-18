// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;


    public class TopicSubscriptionEntity :
        TopicSubscription,
        TopicSubscriptionHandle
    {
        readonly TopicEntity _destination;
        readonly TopicEntity _source;

        public TopicSubscriptionEntity(long id, TopicEntity source, TopicEntity destination)
        {
            Id = id;
            _source = source;
            _destination = destination;
        }

        public static IEqualityComparer<TopicSubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<TopicSubscriptionEntity> EntityComparer { get; } = new ConsumerEntityEqualityComparer();

        public Topic Source => _source.Topic;
        public Topic Destination => _destination.Topic;

        public long Id { get; }
        public TopicSubscription TopicSubscription => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"source: {Source.EntityName}",
                $"destination: {Destination.EntityName}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class ConsumerEntityEqualityComparer : IEqualityComparer<TopicSubscriptionEntity>
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

                return x._source.Equals(y._source) && x._destination.Equals(y._destination);
            }

            public int GetHashCode(TopicSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._source.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._destination.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer : IEqualityComparer<TopicSubscriptionEntity>
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

                return string.Equals(x._source.EntityName, y._source.EntityName);
            }

            public int GetHashCode(TopicSubscriptionEntity obj)
            {
                return obj._source.EntityName.GetHashCode();
            }
        }
    }
}
