// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.RabbitMqTransport.Configuration;


    /// <summary>
    /// Configures the transport for a service bus instance to use RabbitMQ, including
    /// the ReceiveFrom address
    /// </summary>
    public class RabbitMqTransportConfigurator :
        ITransportConfigurator
    {
        readonly IRabbitMqBusFactoryConfigurator _configurator;
        readonly IRabbitMqHost _host;

        public RabbitMqTransportConfigurator(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            _configurator = configurator;
            _host = host;
        }

        public void Configure(string queueName, int consumerLimit, Action<IRabbitMqReceiveEndpointConfigurator> callback)
        {
            _configurator.ReceiveEndpoint(_host, queueName, x =>
            {
                x.PrefetchCount = (ushort)consumerLimit;

                callback(x);
            });
        }
    }
}