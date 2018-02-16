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
namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;
    using MassTransit.Pipeline;


    public class ActiveMqEndpointConfiguration :
        EndpointConfiguration,
        IActiveMqEndpointConfiguration
    {
        readonly IActiveMqTopologyConfiguration _topologyConfiguration;

        public ActiveMqEndpointConfiguration(IActiveMqTopologyConfiguration topologyConfiguration, IConsumePipe consumePipe = null)
            : base(topologyConfiguration, consumePipe)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        ActiveMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IActiveMqTopologyConfiguration topologyConfiguration,
            IConsumePipe consumePipe = null)
            : base(parentConfiguration, topologyConfiguration, consumePipe)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public new IActiveMqTopologyConfiguration Topology => _topologyConfiguration;

        public IActiveMqEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new ActiveMqTopologyConfiguration(_topologyConfiguration);

            return new ActiveMqEndpointConfiguration(this, topologyConfiguration);
        }
    }
}