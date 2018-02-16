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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes.Agents;
    using Integration;
    using Pipeline;
    using Topology;
    using Transports;


    public class SendTransportProvider :
        ISendTransportProvider
    {
        readonly IRabbitMqBusConfiguration _busConfiguration;

        public SendTransportProvider(IRabbitMqBusConfiguration busConfiguration)
        {
            _busConfiguration = busConfiguration;
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return Task.FromResult(GetSendTransport(address));
        }

        ISendTransport GetSendTransport(Uri address)
        {
            if (!_busConfiguration.TryGetHost(address, out var hostConfiguration))
                throw new EndpointNotFoundException($"The host was not found for the specified address: {address}");

            var host = hostConfiguration.Host;

            var settings = host.Topology.SendTopology.GetSendSettings(address);

            var brokerTopology = settings.GetBrokerTopology();

            IAgent<ModelContext> modelSource = GetModelSource(host);

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology);

            var transport = new RabbitMqSendTransport(modelSource, configureTopologyFilter, settings.ExchangeName);

            host.Add(transport);

            return transport;
        }

        protected virtual IAgent<ModelContext> GetModelSource(IRabbitMqHostControl host)
        {
            return new RabbitMqModelCache(host, host.ConnectionCache);
        }
    }
}