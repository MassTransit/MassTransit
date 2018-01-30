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
    using Topology.Configuration.Configurators;
    using Transport;
    using Transports;


    public class ServiceBusReceiveEndpointSpecification :
        ServiceBusEndpointSpecification,
        IServiceBusReceiveEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly BusHostCollection<ServiceBusHost> _hosts;
        readonly IServiceBusSendTopology _sendTopology;
        readonly ReceiveEndpointSettings _settings;
        bool _subscribeMessageTopics;

        public ServiceBusReceiveEndpointSpecification(BusHostCollection<ServiceBusHost> hosts, ServiceBusHost host, string queueName,
            IServiceBusEndpointConfiguration configuration)
            : this(hosts, host, new ReceiveEndpointSettings(queueName, new QueueConfigurator(queueName)), configuration)
        {
        }

        public ServiceBusReceiveEndpointSpecification(BusHostCollection<ServiceBusHost> hosts, ServiceBusHost host, ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration configuration)
            : base(host, settings, settings.QueueConfigurator, configuration)
        {
            _hosts = hosts;
            _settings = settings;
            _configuration = configuration;
            _subscribeMessageTopics = true;

            _settings.QueueConfigurator.BasePath = host.Address.AbsolutePath;
            _sendTopology = configuration.Topology.Send;
        }

        public bool SubscribeMessageTopics
        {
            set => _subscribeMessageTopics = value;
        }

        public bool EnableExpress
        {
            set
            {
                _settings.QueueConfigurator.EnableExpress = value;

                Changed(nameof(EnableExpress));
            }
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set => _settings.QueueConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _settings.QueueConfigurator.RequiresDuplicateDetection = true;
            _settings.QueueConfigurator.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        public bool EnablePartitioning
        {
            set => _settings.QueueConfigurator.EnablePartitioning = value;
        }

        public bool IsAnonymousAccessible
        {
            set => _settings.QueueConfigurator.IsAnonymousAccessible = value;
        }

        public int MaxSizeInMegabytes
        {
            set => _settings.QueueConfigurator.MaxSizeInMegabytes = value;
        }

        public bool RequiresDuplicateDetection
        {
            set => _settings.QueueConfigurator.RequiresDuplicateDetection = value;
        }

        public bool SupportOrdering
        {
            set => _settings.QueueConfigurator.SupportOrdering = value;
        }

        public bool RemoveSubscriptions
        {
            set => _settings.RemoveSubscriptions = value;
        }

        public override void SelectBasicTier()
        {
            base.SelectBasicTier();

            _subscribeMessageTopics = false;
        }

        TimeSpan IServiceBusQueueEndpointConfigurator.MessageWaitTimeout
        {
            set => _settings.MessageWaitTimeout = value;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_settings.PrefetchCount < 0)
                yield return this.Failure("PrefetchCount", "must be >= 0");

            if (_settings.MaxConcurrentCalls <= 0)
                yield return this.Failure("MaxConcurrentCalls", "must be > 0");

            foreach (var result in _settings.QueueConfigurator.Validate())
                yield return result;
        }

        public override void Apply(IBusBuilder builder)
        {
            var receiveEndpointBuilder = new ServiceBusReceiveEndpointBuilder(_hosts, Host, _subscribeMessageTopics, _configuration);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointTopology.BrokerTopology, _settings.RemoveSubscriptions));

            ApplyReceiveEndpoint(builder, receivePipe, receiveEndpointTopology);
        }

        protected override IErrorTransport CreateErrorTransport(ServiceBusHost host)
        {
            var settings = _sendTopology.GetErrorSettings(_settings.QueueConfigurator);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(ServiceBusHost host)
        {
            var settings = _sendTopology.GetDeadLetterSettings(_settings.QueueConfigurator);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings, IPipe<NamespaceContext> namespacePipe)
        {
            return new QueueSendEndpointContextFactory(host.MessagingFactoryCache, host.NamespaceCache, Pipe.Empty<MessagingFactoryContext>(), namespacePipe, settings);
        }

        protected override IClientCache CreateClientCache(Uri inputAddress, IMessagingFactoryCache messagingFactoryCache, INamespaceCache namespaceCache)
        {
            return new ClientCache(inputAddress,
                new QueueClientContextFactory(messagingFactoryCache, namespaceCache, MessagingFactoryPipeConfigurator.Build(), NamespacePipeConfigurator.Build(), _settings));
        }
    }
}