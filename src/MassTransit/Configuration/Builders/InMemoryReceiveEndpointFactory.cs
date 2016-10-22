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
namespace MassTransit.Builders
{
    using System;
    using Configurators;
    using EndpointConfigurators;


    public class InMemoryReceiveEndpointFactory :
        IReceiveEndpointFactory
    {
        readonly InMemoryBusBuilder _builder;

        public InMemoryReceiveEndpointFactory(InMemoryBusBuilder builder)
        {
            _builder = builder;
        }

        public IReceiveEndpoint CreateReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configure)
        {
            var consumePipe = _builder.CreateConsumePipe();

            var busEndpointConfigurator = new InMemoryReceiveEndpointConfigurator(queueName, consumePipe);

            configure?.Invoke(busEndpointConfigurator);

            var configurationResult = BusConfigurationResult.CompileResults(busEndpointConfigurator.Validate());

            var endpointBuilder = new InMemoryEndpointBuilder(_builder);

            busEndpointConfigurator.Apply(endpointBuilder);


            return endpointBuilder.ReceiveEndpoint;
        }
    }
}