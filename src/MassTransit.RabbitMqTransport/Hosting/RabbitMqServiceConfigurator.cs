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
namespace MassTransit.RabbitMqTransport.Hosting
{
    using System;
    using Builders;
    using MassTransit.Hosting;
    using PipeConfigurators;


    /// <summary>
    /// A hosted service can specify receive endpoints using the service configurator
    /// </summary>
    public class RabbitMqServiceConfigurator :
        IServiceConfigurator
    {
        readonly IRabbitMqBusFactoryConfigurator _configurator;
        readonly int _defaultConsumerLimit;
        readonly IRabbitMqHost _host;

        public RabbitMqServiceConfigurator(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            _configurator = configurator;
            _host = host;
            _defaultConsumerLimit = Environment.ProcessorCount * 4;
        }

        public void ReceiveEndpoint(string queueName, int consumerLimit, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator.ReceiveEndpoint(_host, queueName, x =>
            {
                x.PrefetchCount = (ushort)consumerLimit;

                configureEndpoint(x);
            });
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification) where T : class
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            _configurator.AddBusFactorySpecification(configurator);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, _defaultConsumerLimit, configureEndpoint);
        }
    }
}