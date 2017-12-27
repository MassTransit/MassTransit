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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using Builders;
    using Configurators;
    using MassTransit.Configurators;
    using Specifications;


    public class RabbitMqReceiveEndpointFactory :
        IRabbitMqReceiveEndpointFactory
    {
        readonly RabbitMqBusBuilder _builder;
        readonly IRabbitMqHost _host;
        readonly IRabbitMqEndpointConfiguration _configuration;

        public RabbitMqReceiveEndpointFactory(RabbitMqBusBuilder builder, IRabbitMqHost host, IRabbitMqEndpointConfiguration configuration)
        {
            _builder = builder;
            _host = host;
            _configuration = configuration;
        }

        public void CreateReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var endpointConfigurator = new RabbitMqReceiveEndpointSpecification(_host, endpointTopologySpecification, queueName);

            configure?.Invoke(endpointConfigurator);

            BusConfigurationResult.CompileResults(endpointConfigurator.Validate());

            endpointConfigurator.Apply(_builder);
        }
    }
}