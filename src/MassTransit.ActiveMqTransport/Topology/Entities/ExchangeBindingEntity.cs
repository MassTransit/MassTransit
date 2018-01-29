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
namespace MassTransit.ActiveMqTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;


    public class ExchangeBindingEntity :
        ExchangeToExchangeBinding,
        ExchangeBindingHandle
    {
        readonly TopicEntity _destination;

        readonly TopicEntity _source;

        public ExchangeBindingEntity(long id, TopicEntity source, TopicEntity destination, string routingKey)
        {
            Id = id;
            RoutingKey = routingKey;
            _source = source;
            _destination = destination;
        }

        public static IEqualityComparer<ExchangeBindingEntity> EntityComparer { get; } = new ExchangeBindingEntityEqualityComparer();
        public long Id { get; }
        public ExchangeToExchangeBinding Binding => this;

        public Topic Source => _source.Topic;
        public Topic Destination => _destination.Topic;
        public string RoutingKey { get; }

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"source: {Source.EntityName}",
                $"destination: {Destination.EntityName}",
                string.IsNullOrWhiteSpace(RoutingKey) ? "" : $"routing-key: {RoutingKey}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class ExchangeBindingEntityEqualityComparer : IEqualityComparer<ExchangeBindingEntity>
        {
            public bool Equals(ExchangeBindingEntity x, ExchangeBindingEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return x._source.Equals(y._source) && x._destination.Equals(y._destination) && string.Equals(x.RoutingKey, y.RoutingKey);
            }

            public int GetHashCode(ExchangeBindingEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._source.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._destination.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.RoutingKey.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}