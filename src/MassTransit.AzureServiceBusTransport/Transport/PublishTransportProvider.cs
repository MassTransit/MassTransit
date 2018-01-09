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
    using System.Linq;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Transports;


    public class PublishTransportProvider :
        IPublishTransportProvider
    {
        readonly BusHostCollection<ServiceBusHost> _hosts;
        readonly IServiceBusPublishTopology _publishTopology;

        public PublishTransportProvider(BusHostCollection<ServiceBusHost> hosts, IServiceBusHost host)
        {
            _hosts = hosts;
            _publishTopology = host.Topology.PublishTopology;
        }

        Task<ISendTransport> IPublishTransportProvider.GetPublishTransport<T>(Uri publishAddress)
        {
            return Task.FromResult(GetSendTransport<T>(publishAddress));
        }

        ISendTransport GetSendTransport<T>(Uri address)
            where T : class
        {
            var host = GetMatchingHost(address);

            var settings = _publishTopology.GetMessageTopology<T>().GetSendSettings();

            IAgent<SendEndpointContext> source = GetSendEndpointContextSource(host, settings, settings.GetBrokerTopology());

            var transport = new ServiceBusSendTransport(source, address);

            host.Add(transport);

            return transport;
        }

        protected virtual IAgent<SendEndpointContext> GetSendEndpointContextSource(ServiceBusHost host, SendSettings settings, BrokerTopology brokerTopology)
        {
            IPipe<NamespaceContext> pipe = Pipe.New<NamespaceContext>(x => x.UseFilter(new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology, false)));

            var contextFactory = new TopicSendEndpointContextFactory(host.MessagingFactoryCache, host.NamespaceCache, Pipe.Empty<MessagingFactoryContext>(), pipe, settings);

            return new SendEndpointContextCache(contextFactory);
        }

        ServiceBusHost GetMatchingHost(Uri address)
        {
            var host = _hosts.GetHosts(address)
                .OrderByDescending(x => address.AbsolutePath.StartsWith(x.Settings.ServiceUri.AbsolutePath, StringComparison.OrdinalIgnoreCase)
                    ? 1
                    : 0)
                .FirstOrDefault();

            if (host == null)
                throw new EndpointNotFoundException($"The host was not found for the specified address: {address}");

            return host;
        }
    }
}