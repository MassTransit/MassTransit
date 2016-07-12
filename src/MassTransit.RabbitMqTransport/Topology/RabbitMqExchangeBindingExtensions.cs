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
    using Newtonsoft.Json.Linq;
    using Transports;


    public static class RabbitMqExchangeBindingExtensions
    {
        public static IEnumerable<ExchangeBindingSettings> GetExchangeBindings(this Type messageType, IMessageNameFormatter messageNameFormatter, IExchangeTypeProvider exchangeTypeProvider, IRoutingKeyFormatter routingKeyFormatter)
        {
            if (!IsBindableMessageType(messageType))
                yield break;

            bool temporary = IsTemporaryMessageType(messageType);

            var exchangeName = messageNameFormatter.GetMessageName(messageType).ToString();

            var exchange = new Exchange(exchangeName, !temporary, temporary, exchangeTypeProvider);
            
            var routingkey = routingKeyFormatter.CreateRoutingKeyForType(messageType);
            
            var binding = new ExchangeBinding(exchange, routingkey);

            yield return binding;
        }

        static bool IsBindableMessageType(Type messageType)
        {
            if (typeof(JToken) == messageType)
                return false;

            return true;
        }

        public static ExchangeBindingSettings GetErrorExchangeBinding(this SendSettings settings)
        {
            var exchange = new Exchange(settings.ExchangeName, true, false);

            var binding = new ExchangeBinding(exchange);

            return binding;
        }

        public static ExchangeBindingSettings GetExchangeBinding(this SendSettings settings, string exchangeName)
        {
            var exchange = new Exchange(exchangeName, settings.Durable, settings.AutoDelete);

            var binding = new ExchangeBinding(exchange);

            return binding;
        }

        public static IEnumerable<ExchangeBindingSettings> GetExchangeBindings(this ReceiveSettings settings, string exchangeName)
        {
            var exchange = new Exchange(exchangeName, settings.Durable, settings.AutoDelete, settings.ExchangeTypeProvider);

            var binding = new ExchangeBinding(exchange);

            yield return binding;
        }

        public static bool IsTemporaryMessageType(this Type messageType)
        {
            return (!messageType.IsPublic && messageType.IsClass)
                || (messageType.IsGenericType && messageType.GetGenericArguments().Any(IsTemporaryMessageType));
        }


        class Exchange :
            ExchangeSettings
        {
            public Exchange(string exchangeName, bool durable, bool autoDelete, IExchangeTypeProvider exchangeTypeProvider = null)
            {
                ExchangeName = exchangeName;
                Durable = durable;
                AutoDelete = autoDelete;
                ExchangeTypeProvider = exchangeTypeProvider ?? new MasstransitExchangeTypeProvider();
                ExchangeType = ExchangeTypeProvider.GetTypeForExchangeName(exchangeName);
                Arguments = new Dictionary<string, object>();
            }

            public string ExchangeName { get; }
            public string ExchangeType { get; }
            public IExchangeTypeProvider ExchangeTypeProvider { get; }
            public bool Durable { get; }
            public bool AutoDelete { get; }
            public IDictionary<string, object> Arguments { get; }
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
        }
    }
}