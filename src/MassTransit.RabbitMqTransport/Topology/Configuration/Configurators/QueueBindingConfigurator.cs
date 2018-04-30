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


    public class QueueBindingConfigurator :
        QueueConfigurator,
        IQueueBindingConfigurator
    {
        public QueueBindingConfigurator(string queueName, string exchangeType, bool durable, bool autoDelete)
            : base(queueName, exchangeType, durable, autoDelete)
        {
            BindingArguments = new Dictionary<string, object>();
            RoutingKey = "";
        }

        public IDictionary<string, object> BindingArguments { get; }

        public void SetBindingArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                BindingArguments.Remove(key);
            else
                BindingArguments[key] = value;
        }

        public string RoutingKey { get; set; }
    }
}