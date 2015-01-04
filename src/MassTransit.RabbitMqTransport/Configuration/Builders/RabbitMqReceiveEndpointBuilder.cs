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
namespace MassTransit.RabbitMqTransport.Configuration.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EndpointConfigurators;
    using MassTransit.Pipeline;
    using Pipeline;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        IReceiveEndpointBuilder
    {
        readonly IConsumePipe _consumePipe;
        readonly List<ExchangeBindingSettings> _exchangeBindings;
        readonly IMessageNameFormatter _messageNameFormatter;

        public RabbitMqReceiveEndpointBuilder(IConsumePipe consumePipe, IMessageNameFormatter messageNameFormatter)
        {
            _consumePipe = consumePipe;
            _messageNameFormatter = messageNameFormatter;
            _exchangeBindings = new List<ExchangeBindingSettings>();
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            _exchangeBindings.AddRange(GetExchangeBindingSettingses(typeof(T)));

            return _consumePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _consumePipe.ConnectConsumeMessageObserver(observer);
        }

        IConsumePipe IReceiveEndpointBuilder.InputPipe
        {
            get { return _consumePipe; }
        }

        public IEnumerable<ExchangeBindingSettings> GetExchangeBindings()
        {
            return _exchangeBindings;
        }

        IEnumerable<ExchangeBindingSettings> GetExchangeBindingSettingses(Type messageType)
        {
            bool temporary = IsTemporaryMessageType(messageType);

            var exchange = new Exchange(_messageNameFormatter.GetMessageName(messageType).ToString(), !temporary, temporary);

            var binding = new ExchangeBinding(exchange);

            yield return binding;
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