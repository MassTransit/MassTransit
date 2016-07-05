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
    using Configuration;


    public class RabbitMqReceiveSettings :
        ReceiveSettings,
        IQueueConfigurator,
        IExchangeBindingConfigurator
    {
        public RabbitMqReceiveSettings()
        {
            QueueArguments = new Dictionary<string, object>();
            ExchangeArguments = new Dictionary<string, object>();
            ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;

            PrefetchCount = (ushort)Math.Min(Environment.ProcessorCount * 2, 16);

            Durable = true;
            Exclusive = false;
        }

        public RabbitMqReceiveSettings(ReceiveSettings settings)
        {
            QueueName = settings.QueueName;
            ExchangeName = settings.ExchangeName;
            PrefetchCount = settings.PrefetchCount;
            Durable = settings.Durable;
            Exclusive = settings.Exclusive;
            AutoDelete = settings.AutoDelete;
            PurgeOnStartup = settings.PurgeOnStartup;
            ExchangeTypeDeterminer = settings.ExchangeTypeDeterminer ?? new MasstransitExchangeTypeDeterminer();
            RoutingKeyFormatter = settings.RoutingKeyFormatter ?? new MasstransitRoutingKeyFormatter();
            ExchangeType = settings.ExchangeType ?? ExchangeTypeDeterminer.GetTypeForExchangeName(settings.ExchangeName ?? settings.QueueName);
            QueueArguments = new Dictionary<string, object>(settings.QueueArguments);
            ExchangeArguments = new Dictionary<string, object>(settings.ExchangeArguments);
        }

        public string RoutingKey { get; set; }
        public IRoutingKeyFormatter RoutingKeyFormatter { get; set; }

        public void SetQueueArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                QueueArguments.Remove(key);
            else
                QueueArguments[key] = value;
        }

        public bool Lazy
        {
            set { SetQueueArgument("x-queue-mode", value ? "lazy" : "default"); }
        }

        public void SetExchangeArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                ExchangeArguments.Remove(key);
            else
                ExchangeArguments[key] = value;
        }

        public void EnablePriority(byte maxPriority)
        {
            QueueArguments["x-max-priority"] = (int)maxPriority;
        }

        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public ushort PrefetchCount { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> QueueArguments { get; }
        public IDictionary<string, object> ExchangeArguments { get; }
        public bool PurgeOnStartup { get; set; }
        public string ExchangeType { get; set; }
        public IExchangeTypeDeterminer ExchangeTypeDeterminer { get; set; }
    }
}