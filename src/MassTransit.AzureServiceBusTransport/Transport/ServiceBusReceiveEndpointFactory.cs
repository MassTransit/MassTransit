// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class ServiceBusReceiveEndpointFactory :
        IServiceBusReceiveEndpointFactory
    {
        readonly ServiceBusBusBuilder _builder;
        readonly ServiceBusHost _host;

        public ServiceBusReceiveEndpointFactory(ServiceBusBusBuilder builder, ServiceBusHost host)
        {
            _builder = builder;
            _host = host;
        }

        public void CreateReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            var consumePipe = _builder.CreateConsumePipe();

            var endpointConfigurator = new ServiceBusReceiveEndpointSpecification(_host, queueName);

            configure?.Invoke(endpointConfigurator);

            BusConfigurationResult.CompileResults(endpointConfigurator.Validate());

            endpointConfigurator.Apply(_builder);
        }
    }
}