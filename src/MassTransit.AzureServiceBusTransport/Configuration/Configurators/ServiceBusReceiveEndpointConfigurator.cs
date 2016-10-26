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
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Pipeline;
    using Settings;


    public class ServiceBusReceiveEndpointConfigurator :
        ServiceBusEndpointConfigurator,
        IServiceBusReceiveEndpointConfigurator
    {
        readonly ReceiveEndpointSettings _settings;
        bool _subscribeMessageTopics;

        public ServiceBusReceiveEndpointConfigurator(IServiceBusHost host, string queueName, IConsumePipe consumePipe = null)
            : this(host, new ReceiveEndpointSettings(Defaults.CreateQueueDescription(queueName)), consumePipe)
        {
        }

        public ServiceBusReceiveEndpointConfigurator(IServiceBusHost host, ReceiveEndpointSettings settings, IConsumePipe consumePipe = null)
            : base(host, settings, consumePipe)
        {
            _settings = settings;
            _subscribeMessageTopics = true;
        }

        public bool SubscribeMessageTopics
        {
            set { _subscribeMessageTopics = value; }
        }

        public bool EnableExpress
        {
            set
            {
                _settings.QueueDescription.EnableExpress = value;

                Changed("EnableExpress");
            }
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set { _settings.DuplicateDetectionHistoryTimeWindow = value; }
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _settings.RequiresDuplicateDetection = true;
            _settings.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        public bool EnablePartitioning
        {
            set { _settings.EnablePartitioning = value; }
        }

        public bool IsAnonymousAccessible
        {
            set { _settings.IsAnonymousAccessible = value; }
        }

        public int MaxSizeInMegabytes
        {
            set { _settings.MaxSizeInMegabytes = value; }
        }

        public bool RequiresDuplicateDetection
        {
            set { _settings.RequiresDuplicateDetection = value; }
        }

        public bool SupportOrdering
        {
            set { _settings.SupportOrdering = value; }
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_settings.PrefetchCount <= 0)
                yield return this.Failure("PrefetchCount", "must be > 0");
            if (_settings.MaxConcurrentCalls <= 0)
                yield return this.Failure("MaxConcurrentCalls", "must be > 0");
            if (_settings.QueueDescription.AutoDeleteOnIdle != TimeSpan.Zero && _settings.QueueDescription.AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public override void Apply(IBusBuilder builder)
        {
            ServiceBusReceiveEndpointBuilder endpointBuilder = null;
            var receivePipe = CreateReceivePipe(builder, consumePipe =>
            {
                endpointBuilder = new ServiceBusReceiveEndpointBuilder(consumePipe, Host.MessageNameFormatter, _subscribeMessageTopics);
                return endpointBuilder;
            });

            if (endpointBuilder == null)
                throw new InvalidOperationException("The endpoint builder was not initialized");

            ApplyReceiveEndpoint(builder, receivePipe,
                new PrepareReceiveEndpointFilter(_settings, endpointBuilder.GetTopicSubscriptions().ToArray()),
                new PrepareQueueClientFilter(_settings));
        }

        protected override ReceiveEndpointSettings GetReceiveEndpointSettings(string queueName)
        {
            var description = new QueueDescription(queueName)
            {
                AutoDeleteOnIdle = _settings.QueueDescription.AutoDeleteOnIdle,
                EnableExpress = _settings.QueueDescription.EnableExpress,
                DefaultMessageTimeToLive = _settings.QueueDescription.DefaultMessageTimeToLive,
                MaxDeliveryCount = _settings.QueueDescription.MaxDeliveryCount,
                RequiresSession = _settings.QueueDescription.RequiresSession,
                EnablePartitioning = _settings.QueueDescription.EnablePartitioning
            };

            return new ReceiveEndpointSettings(description);
        }
    }
}