// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq.Configuration.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PipeBuilders;
    using Pipeline;


    public class SubscriptionBuilder
    {
        readonly Type _messageType;
        MessageName _messageName;

        public SubscriptionBuilder(Type messageType, IMessageNameFormatter messageNameFormatter)
        {
            _messageType = messageType;
            _messageName = messageNameFormatter.GetMessageName(messageType);
        }

        public void Configure(IPipeBuilder<ModelContext> builder)
        {
            bool temporary = IsTemporaryMessageType(_messageType);
            var exchange = new Exchange(_messageName.ToString(), !temporary, temporary);

            var subscription = new Subscription(exchange, "");

            builder.AddFilter(new SubscriptionModelFilter(subscription));
        }


        static bool IsTemporaryMessageType(Type messageType)
        {
            return (!messageType.IsPublic && messageType.IsClass)
                   || (messageType.IsGenericType
                       && messageType.GetGenericArguments().Any(IsTemporaryMessageType));
        }


        class Exchange :
            ExchangeSettings
        {
            public Exchange(string exchangeName, bool durable, bool autoDelete)
            {
                ExchangeName = exchangeName;
                Durable = durable;
                AutoDelete = autoDelete;
            }

            public string ExchangeName { get; private set; }
            public string ExchangeType { get; private set; }
            public bool Durable { get; private set; }
            public bool AutoDelete { get; private set; }
            public IDictionary<string, object> Arguments { get; private set; }
        }


        class Subscription :
            SubscriptionSettings
        {
            public Subscription(ExchangeSettings exchange, string routingKey)
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