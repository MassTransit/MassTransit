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
    using System.Threading.Tasks;
    using Integration;
    using Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly BusHostCollection<RabbitMqHost> _hosts;

        public RabbitMqSendTransportProvider(BusHostCollection<RabbitMqHost> hosts)
        {
            _hosts = hosts;
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            var host = _hosts.GetHost(address);

            var sendSettings = host.Topology.GetSendSettings(address);

            var topology = host.Topology.SendTopology.GetBrokerTopology(address);

            var modelCache = new RabbitMqModelCache(host);

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(sendSettings, topology);

            return Task.FromResult<ISendTransport>(new RabbitMqSendTransport(modelCache, configureTopologyFilter, sendSettings.ExchangeName));
        }
    }
}