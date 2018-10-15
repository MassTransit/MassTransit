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
namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using MassTransit.Configuration;


    public class ServiceBusEndpointConfiguration :
        EndpointConfiguration,
        IServiceBusEndpointConfiguration
    {
        readonly IServiceBusTopologyConfiguration _topologyConfiguration;

        public ServiceBusEndpointConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        ServiceBusEndpointConfiguration(IServiceBusEndpointConfiguration parentConfiguration, IServiceBusTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public new IServiceBusTopologyConfiguration Topology => _topologyConfiguration;

        public IServiceBusEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new ServiceBusTopologyConfiguration(_topologyConfiguration);

            return new ServiceBusEndpointConfiguration(this, topologyConfiguration);
        }
    }
}