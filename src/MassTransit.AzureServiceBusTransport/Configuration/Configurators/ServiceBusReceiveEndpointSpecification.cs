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
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using MassTransit.Builders;
    using Pipeline;
    using Settings;
    using Specifications;
    using Topology.Configuration.Configurators;
    using Transport;


    public class ServiceBusReceiveEndpointSpecification :
        ServiceBusEndpointSpecification,
        IServiceBusReceiveEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _configuration;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly ReceiveEndpointSettings _settings;
        bool _subscribeMessageTopics;

        public ServiceBusReceiveEndpointSpecification(IServiceBusHost host, string queueName, IServiceBusEndpointConfiguration configuration,
            ISendTransportProvider sendTransportProvider)
            : this(host, new ReceiveEndpointSettings(new QueueConfigurator(queueName)), configuration, sendTransportProvider)
        {
        }

        public ServiceBusReceiveEndpointSpecification(IServiceBusHost host, ReceiveEndpointSettings settings, IServiceBusEndpointConfiguration configuration,
            ISendTransportProvider sendTransportProvider)
            : base(host, settings, settings.QueueConfigurator, configuration)
        {
            _settings = settings;
            _configuration = configuration;
            _sendTransportProvider = sendTransportProvider;
            _subscribeMessageTopics = true;
        }

        public bool SubscribeMessageTopics
        {
            set => _subscribeMessageTopics = value;
        }

        public bool EnableExpress
        {
            set
            {
                _settings.QueueDescription.EnableExpress = value;

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
            if (_settings.QueueDescription.AutoDeleteOnIdle != TimeSpan.Zero && _settings.QueueDescription.AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public override void Apply(IBusBuilder builder)
        {
            var receiveEndpointBuilder = new ServiceBusReceiveEndpointBuilder(builder, Host, _subscribeMessageTopics, _configuration, _sendTransportProvider);

            var receivePipe = CreateReceivePipe(receiveEndpointBuilder);

            var receiveEndpointTopology = receiveEndpointBuilder.CreateReceiveEndpointTopology(InputAddress, _settings);

            ApplyReceiveEndpoint(receivePipe, receiveEndpointTopology, x =>
            {
                x.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointTopology.BrokerTopology, _settings.RemoveSubscriptions));
                x.UseFilter(new PrepareQueueClientFilter(_settings));
            });
        }
    }
}