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
    using Builders;
    using BusConfigurators;
    using MassTransit.Builders;
    using Settings;
    using Specifications;
    using Topology.Configuration;
    using Topology.Configuration.Configurators;
    using Topology.Topologies;
    using Transports;


    public class ServiceBusBusFactoryConfigurator :
        BusFactoryConfigurator<IBusBuilder>,
        IServiceBusBusFactoryConfigurator,
        IBusFactory
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly BusHostCollection<ServiceBusHost> _hosts;
        readonly QueueConfigurator _queueConfigurator;
        readonly ReceiveEndpointSettings _settings;

        public ServiceBusBusFactoryConfigurator(IServiceBusEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            _hosts = new BusHostCollection<ServiceBusHost>();

            _queueConfigurator = new QueueConfigurator("no-host-configured")
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5),
                EnableExpress = true
            };

            _settings = new ReceiveEndpointSettings("no-host-configured", _queueConfigurator);
        }

        public IBusControl CreateBus()
        {
            var builder = new ServiceBusBusBuilder(_hosts, _settings, _configuration);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set => _queueConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        public bool EnableExpress
        {
            set => _queueConfigurator.EnableExpress = value;
        }

        public bool EnablePartitioning
        {
            set => _queueConfigurator.EnablePartitioning = value;
        }

        public bool IsAnonymousAccessible
        {
            set => _queueConfigurator.IsAnonymousAccessible = value;
        }

        public int MaxSizeInMegabytes
        {
            set => _queueConfigurator.MaxSizeInMegabytes = value;
        }

        public bool RequiresDuplicateDetection
        {
            set => _queueConfigurator.RequiresDuplicateDetection = value;
        }

        public bool SupportOrdering
        {
            set => _queueConfigurator.SupportOrdering = value;
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_hosts.Count == 0)
                throw new ArgumentException("At least one host must be configured before configuring a receive endpoint");

            ReceiveEndpoint(_hosts[0], queueName, configureEndpoint);
        }

        public void OverrideDefaultBusEndpointQueueName(string value)
        {
            _queueConfigurator.Path = value;
        }

        public void Send<T>(Action<IServiceBusMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IServiceBusMessageSendTopologyConfigurator<T> configurator = _configuration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish<T>(Action<IServiceBusMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IServiceBusMessagePublishTopologyConfigurator<T> configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IServiceBusSendTopologyConfigurator SendTopology => _configuration.Topology.Send;
        public new IServiceBusPublishTopologyConfigurator PublishTopology => _configuration.Topology.Publish;

        public IServiceBusHost Host(ServiceBusHostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var hostTopology = new ServiceBusHostTopology(_configuration.Topology);
            var host = new ServiceBusHost(settings, hostTopology);
            _hosts.Add(host);

            if (_hosts.Count == 1)
            {
                var path = host.Topology.CreateTemporaryQueueName("bus");
                _queueConfigurator.Path = path;
                _settings.Name = path;
            }

            return host;
        }

        public int MaxConcurrentCalls
        {
            set
            {
                _settings.MaxConcurrentCalls = value;
                if (_settings.MaxConcurrentCalls > _settings.PrefetchCount)
                    _settings.PrefetchCount = _settings.MaxConcurrentCalls;
            }
        }

        public int PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public void ReceiveEndpoint(IServiceBusHost host, string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            if (!(host is ServiceBusHost serviceBusHost))
                throw new ArgumentException("Must be a ServiceBusHost", nameof(host));

            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var specification = new ServiceBusReceiveEndpointSpecification(_hosts, serviceBusHost, queueName, endpointTopologySpecification);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }

        public void SubscriptionEndpoint<T>(IServiceBusHost host, string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class
        {
            if (!(host is ServiceBusHost serviceBusHost))
                throw new ArgumentException("Must be a ServiceBusHost", nameof(host));

            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var settings = new SubscriptionEndpointSettings(_configuration.Topology.Publish.GetMessageTopology<T>().TopicDescription, subscriptionName);

            var specification = new ServiceBusSubscriptionEndpointSpecification(_hosts, serviceBusHost, settings, endpointTopologySpecification);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }

        public void SubscriptionEndpoint(IServiceBusHost host, string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            if (!(host is ServiceBusHost serviceBusHost))
                throw new ArgumentException("Must be a ServiceBusHost", nameof(host));

            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var specification = new ServiceBusSubscriptionEndpointSpecification(_hosts, serviceBusHost, subscriptionName, topicName, endpointTopologySpecification);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set => _queueConfigurator.AutoDeleteOnIdle = value;
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set => _queueConfigurator.DefaultMessageTimeToLive = value;
        }

        public bool EnableBatchedOperations
        {
            set => _queueConfigurator.EnableBatchedOperations = value;
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set => _queueConfigurator.EnableDeadLetteringOnMessageExpiration = value;
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set => _queueConfigurator.ForwardDeadLetteredMessagesTo = value;
        }

        public TimeSpan LockDuration
        {
            set => _queueConfigurator.LockDuration = value;
        }

        public int MaxDeliveryCount
        {
            set => _queueConfigurator.MaxDeliveryCount = value;
        }

        public bool RequiresSession
        {
            set => _queueConfigurator.RequiresSession = value;
        }

        public string UserMetadata
        {
            set => _queueConfigurator.UserMetadata = value;
        }

        public void SelectBasicTier()
        {
            _settings.SelectBasicTier();
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _queueConfigurator.RequiresDuplicateDetection = true;
            _queueConfigurator.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        TimeSpan IServiceBusQueueEndpointConfigurator.MessageWaitTimeout
        {
            set => _settings.MessageWaitTimeout = value;
        }
    }
}