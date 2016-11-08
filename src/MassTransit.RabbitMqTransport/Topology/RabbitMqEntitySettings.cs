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


    public abstract class RabbitMqEntitySettings
    {
        protected RabbitMqEntitySettings()
        {
            Durable = true;
            ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;

            ExchangeArguments = new Dictionary<string, object>();
        }

        protected RabbitMqEntitySettings(EntitySettings settings)
        {
            Durable = settings.Durable;
            AutoDelete = settings.AutoDelete;

            ExchangeArguments = new Dictionary<string, object>(settings.ExchangeArguments);
        }

        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public IDictionary<string, object> ExchangeArguments { get; }

        public void SetExchangeArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
            {
                ExchangeArguments.Remove(key);
            }
            else
                ExchangeArguments[key] = value;
        }

        protected virtual IEnumerable<string> GetQueryStringOptions()
        {
            if (!Durable)
                yield return "durable=false";
            if (AutoDelete)
                yield return "autodelete=true";
            if (ExchangeType != RabbitMQ.Client.ExchangeType.Fanout)
                yield return "type=" + ExchangeType;
        }
    }
}