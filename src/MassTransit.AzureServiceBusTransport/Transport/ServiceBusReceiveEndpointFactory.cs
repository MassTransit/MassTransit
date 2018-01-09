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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using Builders;
    using Configurators;
    using MassTransit.Configurators;
    using Specifications;
    using Transports;


    public class ServiceBusReceiveEndpointFactory :
        IServiceBusReceiveEndpointFactory
    {
        readonly ServiceBusBusBuilder _builder;
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly ServiceBusHost _host;
        readonly BusHostCollection<ServiceBusHost> _hosts;

        public ServiceBusReceiveEndpointFactory(ServiceBusBusBuilder builder, BusHostCollection<ServiceBusHost> hosts, ServiceBusHost host,
            IServiceBusEndpointConfiguration configuration)
        {
            _builder = builder;
            _host = host;
            _configuration = configuration;
            _hosts = hosts;
        }

        public void CreateReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var specification = new ServiceBusReceiveEndpointSpecification(_hosts, _host, queueName, endpointTopologySpecification);

            configure?.Invoke(specification);

            BusConfigurationResult.CompileResults(specification.Validate());

            specification.Apply(_builder);
        }
    }
}