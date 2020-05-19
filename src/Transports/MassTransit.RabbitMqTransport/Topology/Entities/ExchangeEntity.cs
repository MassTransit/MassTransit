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
namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;


    public class ExchangeEntity :
        Exchange,
        ExchangeHandle
    {
        public ExchangeEntity(long id, string name, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            Id = id;
            ExchangeName = name;
            ExchangeType = type;
            Durable = durable;
            AutoDelete = autoDelete;
            ExchangeArguments = arguments ?? new Dictionary<string, object>();
        }

        public static IEqualityComparer<ExchangeEntity> NameComparer { get; } = new NameEqualityComparer();

        public static IEqualityComparer<ExchangeEntity> EntityComparer { get; } = new ExchangeEntityEqualityComparer();

        public string ExchangeName { get; }
        public string ExchangeType { get; }
        public bool Durable { get; }
        public bool AutoDelete { get; }
        public IDictionary<string, object> ExchangeArguments { get; }
        public long Id { get; }
        public Exchange Exchange => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"name: {ExchangeName}",
                $"type: {ExchangeType}",
                Durable ? "durable" : "",
                AutoDelete ? "auto-delete" : "",
                string.Join(", ", ExchangeArguments.Select(x => $"{x.Key}: {x.Value}"))
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class NameEqualityComparer : IEqualityComparer<ExchangeEntity>
        {
            public bool Equals(ExchangeEntity x, ExchangeEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.ExchangeName, y.ExchangeName);
            }

            public int GetHashCode(ExchangeEntity obj)
            {
                return obj.ExchangeName.GetHashCode();
            }
        }


        sealed class ExchangeEntityEqualityComparer :
            IEqualityComparer<ExchangeEntity>
        {
            public bool Equals(ExchangeEntity x, ExchangeEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.ExchangeName, y.ExchangeName) && string.Equals(x.ExchangeType, y.ExchangeType) && x.Durable == y.Durable && x.AutoDelete == y.AutoDelete
                    && x.ExchangeArguments.All(a => y.ExchangeArguments.TryGetValue(a.Key, out var value) && a.Value.Equals(value));
            }

            public int GetHashCode(ExchangeEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.ExchangeName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ExchangeType.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Durable.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.AutoDelete.GetHashCode();
                    foreach (KeyValuePair<string, object> keyValuePair in obj.ExchangeArguments)
                    {
                        hashCode = (hashCode * 397) ^ keyValuePair.Key.GetHashCode();
                        hashCode = (hashCode * 397) ^ keyValuePair.Value.GetHashCode();
                    }
                    return hashCode;
                }
            }
        }
    }
}