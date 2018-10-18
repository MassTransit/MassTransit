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
namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using Configuration;
    using Context;
    using Topology;
    using Transport;


    public class ServiceBusEntityReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ServiceBusReceiveEndpointContext
    {
        readonly IServiceBusEntityEndpointConfiguration _configuration;

        public ServiceBusEntityReceiveEndpointContext(IServiceBusEntityEndpointConfiguration configuration, BrokerTopology brokerTopology)
            : base(configuration)
        {
            _configuration = configuration;

            BrokerTopology = brokerTopology;
        }

        public BrokerTopology BrokerTopology { get; }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new SendEndpointSendTransportProvider(_configuration.BusConfiguration);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new PublishTransportProvider(_configuration.BusConfiguration);
        }
    }
}