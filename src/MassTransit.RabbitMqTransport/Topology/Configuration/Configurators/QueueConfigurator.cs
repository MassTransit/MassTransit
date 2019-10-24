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


    public class QueueConfigurator :
        ExchangeConfigurator,
        IQueueConfigurator,
        Queue
    {
        public QueueConfigurator(string queueName, string exchangeType, bool durable, bool autoDelete)
            : base(queueName, exchangeType, durable, autoDelete)
        {
            QueueArguments = new Dictionary<string, object>();

            QueueName = queueName;
        }

        public QueueConfigurator(QueueConfigurator source, string name)
            : base(name, source.ExchangeType, source.Durable, source.AutoDelete)
        {
            QueueArguments = new Dictionary<string, object>();

            QueueName = name;
        }

        public void SetQueueArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                QueueArguments.Remove(key);
            else
                QueueArguments[key] = value;
        }

        public void SetQueueArgument(string key, TimeSpan value)
        {
            var milliseconds = (int)value.TotalMilliseconds;

            SetQueueArgument(key, milliseconds);
        }

        public bool Lazy
        {
            set => SetQueueArgument("x-queue-mode", value ? "lazy" : "default");
        }

        public void EnablePriority(byte maxPriority)
        {
            QueueArguments[RabbitMQ.Client.Headers.XMaxPriority] = (int)maxPriority;
        }

        public string QueueName { get; set; }
        public bool Exclusive { get; set; }
        public TimeSpan? QueueExpiration { get; set; }

        public IDictionary<string, object> QueueArguments { get; }
    }
}
