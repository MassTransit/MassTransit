// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using Builders;
    using BusConfigurators;
    using GreenPipes;
    using MassTransit.Builders;
    using Settings;
    using Transports;


    public class ServiceBusBusFactoryConfigurator :
        BusFactoryConfigurator,
        IServiceBusBusFactoryConfigurator,
        IBusFactory
    {
        readonly BusHostCollection<ServiceBusHost> _hosts;
        readonly ReceiveEndpointSettings _settings;
        readonly IList<IBusFactorySpecification> _specifications;

        public ServiceBusBusFactoryConfigurator()
        {
            _hosts = new BusHostCollection<ServiceBusHost>();
            _specifications = new List<IBusFactorySpecification>();

            var queueName = this.GetTemporaryQueueName("bus");
            _settings = new ReceiveEndpointSettings(Defaults.CreateQueueDescription(queueName))
            {
                QueueDescription =
                {
                    EnableExpress = true,
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
                }
            };
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate()
                .Concat(_specifications.SelectMany(x => x.Validate()));
        }

        public IBusControl CreateBus()
        {
            var builder = new ServiceBusBusBuilder(_hosts, ConsumePipeFactory, SendPipeFactory, PublishPipeFactory, _settings);

            foreach (var configurator in _specifications)
                configurator.Apply(builder);

            return builder.Build();
        }

        public void AddBusFactorySpecification(IBusFactorySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
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

        public IServiceBusHost Host(ServiceBusHostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var host = new ServiceBusHost(settings);
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
            set { _settings.PrefetchCount = value; }
        }

        public void ReceiveEndpoint(IServiceBusHost host, string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            var endpointConfigurator = new ServiceBusReceiveEndpointConfigurator(host, queueName);

            configure(endpointConfigurator);

            AddBusFactorySpecification(endpointConfigurator);
        }

        public void SubscriptionEndpoint<T>(IServiceBusHost host, string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class
        {
            SubscriptionEndpoint(host, subscriptionName, host.MessageNameFormatter.GetTopicAddress(host, typeof(T)).AbsolutePath.Trim('/'), configure);
        }

        public void SubscriptionEndpoint(IServiceBusHost host, string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            var endpointSpecification = new ServiceBusSubscriptionEndpointConfigurator(host, subscriptionName, topicName);

            configure?.Invoke(endpointSpecification);

            AddBusFactorySpecification(endpointSpecification);
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set { _settings.QueueDescription.AutoDeleteOnIdle = value; }
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set { _settings.QueueDescription.DefaultMessageTimeToLive = value; }
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set { _settings.QueueDescription.DuplicateDetectionHistoryTimeWindow = value; }
        }

        public bool EnableBatchedOperations
        {
            set { _settings.QueueDescription.EnableBatchedOperations = value; }
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set { _settings.QueueDescription.EnableDeadLetteringOnMessageExpiration = value; }
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _settings.QueueDescription.RequiresDuplicateDetection = true;
            _settings.QueueDescription.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        public bool EnableExpress
        {
            set { _settings.QueueDescription.EnableExpress = value; }
        }

        public bool EnablePartitioning
        {
            set { _settings.QueueDescription.EnablePartitioning = value; }
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set { _settings.QueueDescription.ForwardDeadLetteredMessagesTo = value; }
        }

        public bool IsAnonymousAccessible
        {
            set { _settings.QueueDescription.IsAnonymousAccessible = value; }
        }

        public TimeSpan LockDuration
        {
            set { _settings.QueueDescription.LockDuration = value; }
        }

        public int MaxDeliveryCount
        {
            set { _settings.QueueDescription.MaxDeliveryCount = value; }
        }

        public int MaxSizeInMegabytes
        {
            set { _settings.QueueDescription.MaxSizeInMegabytes = value; }
        }

        public bool RequiresDuplicateDetection
        {
            set { _settings.QueueDescription.RequiresDuplicateDetection = value; }
        }

        public bool RequiresSession
        {
            set { _settings.QueueDescription.RequiresSession = value; }
        }

        public bool SupportOrdering
        {
            set { _settings.QueueDescription.SupportOrdering = value; }
        }

        public string UserMetadata
        {
            set { _settings.QueueDescription.UserMetadata = value; }
        }
    }
}