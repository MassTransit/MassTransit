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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;
    using MassTransit.Pipeline;


    public class RabbitMqEndpointConfiguration :
        EndpointConfiguration,
        IRabbitMqEndpointConfiguration
    {
        readonly IRabbitMqTopologyConfiguration _topologyConfiguration;

        public RabbitMqEndpointConfiguration(IRabbitMqTopologyConfiguration topologyConfiguration, IConsumePipe consumePipe = null)
            : base(topologyConfiguration, consumePipe)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public RabbitMqEndpointConfiguration(IBusConfiguration busConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration,
            IConsumePipe consumePipe = null)
            : base(busConfiguration, topologyConfiguration, consumePipe)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        RabbitMqEndpointConfiguration(IEndpointConfiguration parentConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration,
            IConsumePipe consumePipe = null)
            : base(parentConfiguration, topologyConfiguration, consumePipe)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public new IRabbitMqTopologyConfiguration Topology => _topologyConfiguration;

        public IRabbitMqEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new RabbitMqTopologyConfiguration(_topologyConfiguration);

            return new RabbitMqEndpointConfiguration(this, topologyConfiguration);
        }
    }
}