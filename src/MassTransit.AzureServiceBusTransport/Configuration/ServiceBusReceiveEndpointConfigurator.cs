// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using EndpointConfigurators;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class ServiceBusReceiveEndpointConfigurator :
        ReceiveEndpointConfigurator,
        IServiceBusReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IServiceBusHost _host;
        readonly ReceiveEndpointSettings _settings;

        public ServiceBusReceiveEndpointConfigurator(IServiceBusHost host, string queueName, IConsumePipe consumePipe = null)
            : this(host, new ReceiveEndpointSettings(queueName), consumePipe)
        {
        }

        public ServiceBusReceiveEndpointConfigurator(IServiceBusHost host, ReceiveEndpointSettings settings, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _host = host;
            _settings = settings;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (ValidationResult result in base.Validate())
                yield return result;

            if (_settings.PrefetchCount <= 0)
                yield return this.Failure("PrefetchCount", "must be > 0");
            if (_settings.MaxConcurrentCalls <= 0)
                yield return this.Failure("MaxConcurrentCalls", "must be > 0");
            if (_settings.QueueDescription.AutoDeleteOnIdle != TimeSpan.Zero && _settings.QueueDescription.AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public void Apply(IBusBuilder builder)
        {
            ServiceBusReceiveEndpointBuilder endpointBuilder = null;
            var receivePipe = CreateReceivePipe(builder, consumePipe =>
            {
                endpointBuilder = new ServiceBusReceiveEndpointBuilder(consumePipe, _host.MessageNameFormatter);
                return endpointBuilder;
            });

            if (endpointBuilder == null)
                throw new InvalidOperationException("The endpoint builder was not initialized");

            var transport = new ServiceBusReceiveTransport(_host, _settings, endpointBuilder.GetTopicSubscriptions().ToArray());

            builder.AddReceiveEndpoint(new ReceiveEndpoint(transport, receivePipe));
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

        public bool SupportOrdering
        {
            set { _settings.QueueDescription.SupportOrdering = value; }
        }

        public string UserMetadata
        {
            set { _settings.QueueDescription.UserMetadata = value; }
        }

        public Uri InputAddress => _host.Settings.GetInputAddress(_settings.QueueDescription);

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

        protected override Uri GetErrorAddress()
        {
            string errorQueueName = _settings.QueueDescription.Path + "_error";

            var errorQueueDescription = GetQueueDescription(errorQueueName);

            return _host.Settings.GetInputAddress(errorQueueDescription);
        }

        QueueDescription GetQueueDescription(string errorQueueName)
        {
            return new QueueDescription(errorQueueName)
            {
                AutoDeleteOnIdle = _settings.QueueDescription.AutoDeleteOnIdle,
                EnableExpress = _settings.QueueDescription.EnableExpress,
                DefaultMessageTimeToLive = _settings.QueueDescription.DefaultMessageTimeToLive
            };
        }

        protected override Uri GetDeadLetterAddress()
        {
            string skippedQueueName = _settings.QueueDescription.Path + "_skipped";

            var errorQueueDescription = GetQueueDescription(skippedQueueName);

            return _host.Settings.GetInputAddress(errorQueueDescription);
        }
    }
}