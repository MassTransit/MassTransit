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
namespace MassTransit.AzureServiceBusTransport.Configuration.Configurators
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
    using Transport;
    using Transports;


    public class ServiceBusBusFactoryConfigurator :
        BusFactoryConfigurator<IBusBuilder>,
        IServiceBusBusFactoryConfigurator,
        IBusFactory
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly BusHostCollection<ServiceBusHost> _hosts;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly ReceiveEndpointSettings _settings;

        public ServiceBusBusFactoryConfigurator(IServiceBusEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            _hosts = new BusHostCollection<ServiceBusHost>();

            _sendTransportProvider = new ServiceBusSendTransportProvider(_hosts);

            var queueName = ((IServiceBusHost)null).GetTemporaryQueueName("bus");
            _settings = new ReceiveEndpointSettings(new QueueConfigurator(queueName))
            {
                
                QueueDescription =
                {
                    EnableExpress = true,
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
                }
            };
        }

        public IBusControl CreateBus()
        {
            var builder = new ServiceBusBusBuilder(_hosts, _settings, _configuration, _sendTransportProvider);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set => _settings.QueueDescription.DuplicateDetectionHistoryTimeWindow = value;
        }

        public bool EnableExpress
        {
            set => _settings.QueueDescription.EnableExpress = value;
        }

        public bool EnablePartitioning
        {
            set => _settings.QueueDescription.EnablePartitioning = value;
        }

        public bool IsAnonymousAccessible
        {
            set => _settings.QueueDescription.IsAnonymousAccessible = value;
        }

        public int MaxSizeInMegabytes
        {
            set => _settings.QueueDescription.MaxSizeInMegabytes = value;
        }

        public bool RequiresDuplicateDetection
        {
            set => _settings.QueueDescription.RequiresDuplicateDetection = value;
        }

        public bool SupportOrdering
        {
            set => _settings.QueueDescription.SupportOrdering = value;
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            if (_hosts.Count == 0)
                throw new ArgumentException("At least one host must be configured before configuring a receive endpoint");

            ReceiveEndpoint(_hosts[0], queueName, configureEndpoint);
        }

        public void OverrideDefaultBusEndpointQueueName(string value)
        {
            _settings.QueueDescription.Path = value;
        }

        public void Send<T>(Action<IServiceBusMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            var configurator = _configuration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish<T>(Action<IServiceBusMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            var configurator = _configuration.Topology.Publish.GetMessageTopology<T>();

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
            var serviceBusHost = host as ServiceBusHost;
            if (serviceBusHost == null)
                throw new ArgumentException("Must be a ServiceBusHost", nameof(host));

            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var specification = new ServiceBusReceiveEndpointSpecification(serviceBusHost, queueName, endpointTopologySpecification, _sendTransportProvider);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }

        public void SubscriptionEndpoint<T>(IServiceBusHost host, string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class
        {
            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var settings = new SubscriptionEndpointSettings(_configuration.Topology.Publish.GetMessageTopology<T>().TopicDescription, subscriptionName);

            var specification = new ServiceBusSubscriptionEndpointSpecification(host, settings, endpointTopologySpecification, _sendTransportProvider);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }

        public void SubscriptionEndpoint(IServiceBusHost host, string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            var endpointTopologySpecification = _configuration.CreateNewConfiguration();

            var specification = new ServiceBusSubscriptionEndpointSpecification(host, subscriptionName, topicName, endpointTopologySpecification, _sendTransportProvider);

            specification.ConnectConsumerConfigurationObserver(this);
            specification.ConnectSagaConfigurationObserver(this);

            AddReceiveEndpointSpecification(specification);

            configure?.Invoke(specification);
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set => _settings.QueueDescription.AutoDeleteOnIdle = value;
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set => _settings.QueueDescription.DefaultMessageTimeToLive = value;
        }

        public bool EnableBatchedOperations
        {
            set => _settings.QueueDescription.EnableBatchedOperations = value;
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set => _settings.QueueDescription.EnableDeadLetteringOnMessageExpiration = value;
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set => _settings.QueueDescription.ForwardDeadLetteredMessagesTo = value;
        }

        public TimeSpan LockDuration
        {
            set => _settings.QueueDescription.LockDuration = value;
        }

        public int MaxDeliveryCount
        {
            set => _settings.QueueDescription.MaxDeliveryCount = value;
        }

        public bool RequiresSession
        {
            set => _settings.QueueDescription.RequiresSession = value;
        }

        public string UserMetadata
        {
            set => _settings.QueueDescription.UserMetadata = value;
        }

        public void SelectBasicTier()
        {
            _settings.SelectBasicTier();
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _settings.QueueDescription.RequiresDuplicateDetection = true;
            _settings.QueueDescription.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        TimeSpan IServiceBusQueueEndpointConfigurator.MessageWaitTimeout
        {
            set => _settings.MessageWaitTimeout = value;
        }
    }
}