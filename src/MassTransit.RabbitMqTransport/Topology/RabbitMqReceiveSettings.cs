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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using Configuration.Configurators;


    public class RabbitMqReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        public RabbitMqReceiveSettings(string name, string type, bool durable, bool autoDelete)
            : base(name,type,durable,autoDelete)
        {
            PrefetchCount = (ushort)Math.Min(Environment.ProcessorCount * 2, 16);
        }

        public ushort PrefetchCount { get; set; }
        public bool PurgeOnStartup { get; set; }

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