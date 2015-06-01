// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Transports;


    public static class RabbitMqExchangeBindingExtensions
    {
        public static ExchangeBindingSettings GetExchangeBinding(this Type messageType, IMessageNameFormatter messageNameFormatter)
        {
            bool temporary = IsTemporaryMessageType(messageType);

            var exchange = new Exchange(messageNameFormatter.GetMessageName(messageType).ToString(), !temporary, temporary);

            var binding = new ExchangeBinding(exchange);

            return binding;
        }

        public static ExchangeBindingSettings GetErrorExchangeBinding(this SendSettings settings)
        {
            var exchange = new Exchange(settings.ExchangeName, true, false);

            var binding = new ExchangeBinding(exchange);

            return binding;
        }

        public static bool IsTemporaryMessageType(this Type messageType)
        {
            return (!messageType.IsPublic && messageType.IsClass)
                || (messageType.IsGenericType && messageType.GetGenericArguments().Any(IsTemporaryMessageType));
        }


        class Exchange :
            ExchangeSettings
        {
            public Exchange(string exchangeName, bool durable, bool autoDelete)
            {
                ExchangeName = exchangeName;
                Durable = durable;
                AutoDelete = autoDelete;
                ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;
            }

            public string ExchangeName { get; private set; }
            public string ExchangeType { get; set; }
            public bool Durable { get; private set; }
            public bool AutoDelete { get; private set; }
            public IDictionary<string, object> Arguments { get; private set; }
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

            public ExchangeSettings Exchange { get; private set; }
            public string RoutingKey { get; private set; }
            public IDictionary<string, object> Arguments { get; private set; }
        }
    }
}