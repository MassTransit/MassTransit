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
namespace MassTransit.RabbitMqTransport.Topology.Configurators
{
    using System;
    using System.Collections.Generic;
    using Entities;


    public class ExchangeConfigurator :
        IExchangeConfigurator,
        Exchange
    {
        public ExchangeConfigurator(string exchangeName, string exchangeType, bool durable = true, bool autoDelete = false)
        {
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;

            ExchangeArguments = new Dictionary<string, object>();
        }

        public ExchangeConfigurator(Exchange source)
        {
            ExchangeName = source.ExchangeName;
            ExchangeType = source.ExchangeType;
            Durable = source.Durable;
            AutoDelete = source.AutoDelete;

            ExchangeArguments = new Dictionary<string, object>(source.ExchangeArguments);
        }

        public string ExchangeName { get; set; }

        public IDictionary<string, object> ExchangeArguments { get; }
        public string ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }

        public void SetExchangeArgument(string key, object value)
        {
            if (value != null)
                ExchangeArguments[key] = value;
            else
                ExchangeArguments.Remove(key);
        }

        public void SetExchangeArgument(string key, TimeSpan value)
        {
            var milliseconds = (int)value.TotalMilliseconds;

            SetExchangeArgument(key, milliseconds);
        }

        public virtual RabbitMqEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new RabbitMqEndpointAddress(hostAddress, ExchangeName, ExchangeType, Durable, AutoDelete);
        }
    }
}
