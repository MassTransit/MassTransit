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
namespace RapidTransit
{
    using System;
    using MassTransit;
    using MassTransit.RabbitMqTransport;


    /// <summary>
    /// A hosted service can specify receive endpoints using the service configurator
    /// </summary>
    public class RabbitMqServiceConfigurator :
        IServiceConfigurator
    {
        readonly IRabbitMqBusFactoryConfigurator _configurator;
        readonly IRabbitMqHost _host;

        public RabbitMqServiceConfigurator(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            _configurator = configurator;
            _host = host;
        }

        public void ReceiveEndpoint(string queueName, int consumerLimit, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator.ReceiveEndpoint(_host, queueName, x =>
            {
                x.PrefetchCount = (ushort)consumerLimit;

                configureEndpoint(x);
            });
        }
    }
}