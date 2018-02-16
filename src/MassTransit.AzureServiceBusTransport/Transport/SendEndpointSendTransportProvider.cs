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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Transports;


    public class SendEndpointSendTransportProvider :
        ISendTransportProvider
    {
        readonly IServiceBusBusConfiguration _busConfiguration;

        public SendEndpointSendTransportProvider(IServiceBusBusConfiguration busConfiguration)
        {
            _busConfiguration = busConfiguration;
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            return Task.FromResult(GetSendTransport(address));
        }

        ISendTransport GetSendTransport(Uri address)
        {
            var host = _busConfiguration.GetHost(address);

            var settings = host.Topology.SendTopology.GetSendSettings(address);

            IAgent<SendEndpointContext> source = GetSendEndpointContextSource(host, settings, settings.GetBrokerTopology());

            var transport = new ServiceBusSendTransport(source, address);

            host.Add(transport);

            return transport;
        }

        IAgent<SendEndpointContext> GetSendEndpointContextSource(IServiceBusHost host, SendSettings settings, BrokerTopology brokerTopology)
        {
            IPipe<NamespaceContext> namespacePipe =
                Pipe.New<NamespaceContext>(x => x.UseFilter(new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology, false)));

            var contextFactory = new QueueSendEndpointContextFactory(host.MessagingFactoryCache, host.NamespaceCache, Pipe.Empty<MessagingFactoryContext>(),
                namespacePipe, settings);

            return new SendEndpointContextCache(contextFactory);
        }
    }
}