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
namespace MassTransit.RabbitMqTransport.Topology.Settings
{
    using System;
    using System.Collections.Generic;
    using Configurators;


    public class RabbitMqReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        public RabbitMqReceiveSettings(string name, string type, bool durable, bool autoDelete)
            : base(name, type, durable, autoDelete)
        {
            PrefetchCount = (ushort)Math.Min(Environment.ProcessorCount * 2, 16);

            ConsumeArguments = new Dictionary<string, object>();
        }

        public ushort PrefetchCount { get; set; }
        public bool PurgeOnStartup { get; set; }
        public bool ExclusiveConsumer { get; set; }

        public bool BindQueue { get; set; } = true;

        public int ConsumerPriority
        {
            set => ConsumeArguments[RabbitMQ.Client.Headers.XPriority] = value;
        }

        public IDictionary<string, object> ConsumeArguments { get; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            var builder = new UriBuilder(hostAddress);

            builder.Path = builder.Path == "/"
                ? $"/{QueueName}"
                : $"/{string.Join("/", builder.Path.Trim('/'), QueueName)}";

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }
    }
}
