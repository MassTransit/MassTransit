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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using Configuration;
    using Configuration.Builders;
    using Configuration.Configurators;
    using Configuration.Specifications;
    using MassTransit.Configurators;
    using Settings;


    public class ServiceBusSubscriptionEndpointFactory :
        IServiceBusSubscriptionEndpointFactory
    {
        readonly ServiceBusBusBuilder _builder;
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly ServiceBusHost _host;
        readonly ISendTransportProvider _sendTransportProvider;

        public ServiceBusSubscriptionEndpointFactory(ServiceBusBusBuilder builder, ServiceBusHost host, IServiceBusEndpointConfiguration configuration,
            ISendTransportProvider sendTransportProvider)
        {
            _builder = builder;
            _host = host;
            _configuration = configuration;
            _sendTransportProvider = sendTransportProvider;
        }

        public void CreateSubscriptionEndpoint(SubscriptionEndpointSettings settings, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            var endpointConfiguration = _configuration.CreateNewConfiguration();

            var endpointConfigurator = new ServiceBusSubscriptionEndpointSpecification(_host, settings, endpointConfiguration, _sendTransportProvider);

            configure?.Invoke(endpointConfigurator);

            BusConfigurationResult.CompileResults(endpointConfigurator.Validate());

            endpointConfigurator.Apply(_builder);
        }
    }
}