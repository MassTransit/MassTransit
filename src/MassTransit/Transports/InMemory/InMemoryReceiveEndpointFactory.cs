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
namespace MassTransit.Transports.InMemory
{
    using System;
    using Builders;
    using Configurators;


    public class InMemoryReceiveEndpointFactory :
        IInMemoryReceiveEndpointFactory
    {
        readonly InMemoryBusBuilder _builder;
        readonly IInMemoryEndpointConfiguration _configuration;
        readonly ISendTransportProvider _sendTransportProvider;

        public InMemoryReceiveEndpointFactory(InMemoryBusBuilder builder, ISendTransportProvider sendTransportProvider,
            IInMemoryEndpointConfiguration configuration)
        {
            _builder = builder;
            _sendTransportProvider = sendTransportProvider;
            _configuration = configuration;
        }

        public void CreateReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _configuration.CreateNewConfiguration();

            var specification = new InMemoryReceiveEndpointSpecification(_builder.InMemoryHost.Address, queueName, _sendTransportProvider, endpointConfiguration);

            configure?.Invoke(specification);

            BusConfigurationResult.CompileResults(specification.Validate());

            specification.Apply(_builder);
        }
    }
}