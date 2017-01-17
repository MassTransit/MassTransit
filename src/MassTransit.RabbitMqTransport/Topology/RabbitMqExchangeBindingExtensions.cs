// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class RabbitMqExchangeBindingExtensions
    {
        public static ExchangeBindingSettings GetExchangeBinding(this SendSettings settings, string exchangeName)
        {
            var exchange = new Exchange(exchangeName, settings.Durable, settings.AutoDelete);

            var binding = new ExchangeBinding(exchange);

            return binding;
        }

        public static bool IsTemporaryMessageType(this Type messageType)
        {
            return (!messageType.IsVisible && messageType.IsClass)
                || (messageType.IsGenericType && messageType.GetGenericArguments().Any(IsTemporaryMessageType));
        }


        class Exchange :
            ExchangeSettings
        {
            public Exchange(string exchangeName, bool durable, bool autoDelete, string exchangeType = null)
            {
                ExchangeName = exchangeName;
                Durable = durable;
                AutoDelete = autoDelete;
                ExchangeType = exchangeType ?? RabbitMQ.Client.ExchangeType.Fanout;
                Arguments = new Dictionary<string, object>();
            }

            public string ExchangeName { get; }
            public string ExchangeType { get; }
            public bool Durable { get; }
            public bool AutoDelete { get; }
            public IDictionary<string, object> Arguments { get; }

            protected bool Equals(Exchange other)
            {
                return string.Equals(ExchangeName, other.ExchangeName);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((Exchange)obj);
            }

            public override int GetHashCode()
            {
                return ExchangeName.GetHashCode();
            }
        }


        class ExchangeBinding :
            ExchangeBindingSettings
        {
            public ExchangeBinding(ExchangeSettings exchange, string routingKey = "")
            {
                RoutingKey = routingKey;
                Exchange = exchange;
                Arguments = new Dictionary<string, object>();
            }

            public ExchangeSettings Exchange { get; }
            public string RoutingKey { get; }
            public IDictionary<string, object> Arguments { get; }

            protected bool Equals(ExchangeBinding other)
            {
                return Equals(Exchange, other.Exchange) && string.Equals(RoutingKey, other.RoutingKey);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((ExchangeBinding)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Exchange?.GetHashCode() ?? 0) * 397) ^ (RoutingKey?.GetHashCode() ?? 0);
                }
            }
        }
    }
}