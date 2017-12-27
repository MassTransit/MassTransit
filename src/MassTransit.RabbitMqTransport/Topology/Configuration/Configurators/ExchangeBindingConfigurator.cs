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
namespace MassTransit.RabbitMqTransport.Topology.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;
    using Entities;


    public class ExchangeBindingConfigurator :
        ExchangeConfigurator,
        IExchangeBindingConfigurator
    {
        public ExchangeBindingConfigurator(string exchangeName, string exchangeType, bool durable, bool autoDelete, string routingKey)
            : base(exchangeName, exchangeType, durable, autoDelete)
        {
            RoutingKey = routingKey;

            BindingArguments = new Dictionary<string, object>();
        }

        public ExchangeBindingConfigurator(Exchange exchange, string routingKey)
            : base(exchange)
        {
            RoutingKey = routingKey;
            
            BindingArguments = new Dictionary<string, object>();
        }

        public void SetBindingArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                BindingArguments.Remove(key);
            else
                BindingArguments[key] = value;
        }

        public IDictionary<string, object> BindingArguments { get; }

        public string RoutingKey { get; set; }
    }
}