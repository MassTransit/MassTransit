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
namespace MassTransit.AzureServiceBusTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Builders;
    using Pipeline;
    using Settings;
    using Specifications;
    using Topology;
    using Topology.Configuration;
    using Transport;
    using Transports;


    public class ServiceBusSubscriptionEndpointSpecification :
        ServiceBusEndpointSpecification,
        IServiceBusSubscriptionEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly BusHostCollection<ServiceBusHost> _hosts;
        readonly SubscriptionEndpointSettings _settings;
        readonly IServiceBusSendTopology _sendTopology;

        public ServiceBusSubscriptionEndpointSpecification(BusHostCollection<ServiceBusHost> hosts, ServiceBusHost host, string subscriptionName, string topicName,
            IServiceBusEndpointConfiguration configuration)
            : this(hosts, host, new SubscriptionEndpointSettings(topicName, subscriptionName), configuration)
        {
        }

        public ServiceBusSubscriptionEndpointSpecification(BusHostCollection<ServiceBusHost> hosts, ServiceBusHost host, SubscriptionEndpointSettings settings,
            IServiceBusEndpointConfiguration configuration)
            : base(host, settings, settings.SubscriptionConfigurator, configuration)
        {
            _hosts = hosts;
            _settings = settings;
            _sendTopology = configuration.Topology.Send;
            _configuration = configuration;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public override void Apply(IBusBuilder builder)
        {
            var receiveEndpointBuilder = new ServiceBusSubscriptionEndpointBuilder(_hosts, Host, _configuration);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<SubscriptionSettings>(_settings, receiveEndpointTopology.BrokerTopology, false));

            ApplyReceiveEndpoint(receivePipe, receiveEndpointTopology);
        }

        protected override IErrorTransport CreateErrorTransport(ServiceBusHost host)
        {
            var settings = _sendTopology.GetErrorSettings(_settings.SubscriptionConfigurator);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(ServiceBusHost host)
        {
            var settings = _sendTopology.GetDeadLetterSettings(_settings.SubscriptionConfigurator);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings, IPipe<NamespaceContext> namespacePipe)
        {
            return new QueueSendEndpointContextFactory(host.MessagingFactoryCache, host.NamespaceCache, Pipe.Empty<MessagingFactoryContext>(), namespacePipe, settings);
        }
        protected override IClientCache CreateClientCache(Uri inputAddress, IMessagingFactoryCache messagingFactoryCache, INamespaceCache namespaceCache)
        {
            return new ClientCache(inputAddress,
                new SubscriptionClientContextFactory(messagingFactoryCache, namespaceCache, MessagingFactoryPipeConfigurator.Build(), NamespacePipeConfigurator.Build(),
                    _settings));
        }
    }
}