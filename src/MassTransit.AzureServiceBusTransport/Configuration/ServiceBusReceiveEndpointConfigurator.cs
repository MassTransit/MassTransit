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
        readonly QueueDescription _queueDescription;
        int _maxConcurrentCalls;
        int _prefetchCount;

        public ServiceBusReceiveEndpointConfigurator(IServiceBusHost host, string queueName, IConsumePipe consumePipe = null)
            : this(host, new QueueDescription(queueName), consumePipe)
        {
            _queueDescription.EnableBatchedOperations = true;
            _queueDescription.MaxDeliveryCount = 5;

            DefaultMessageTimeToLive = TimeSpan.FromDays(365);
            LockDuration = TimeSpan.FromMinutes(5);
            EnableDeadLetteringOnMessageExpiration = true;
        }

        public ServiceBusReceiveEndpointConfigurator(IServiceBusHost host, QueueDescription queueDescription, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _host = host;
            _queueDescription = queueDescription;
            _maxConcurrentCalls = Math.Max(Environment.ProcessorCount, 8);
            _prefetchCount = Math.Max(_maxConcurrentCalls, 32);
        }

        public Uri InputAddress
        {
            get { return _host.Settings.GetInputAddress(_queueDescription); }
        }

        public bool DuplicateDetection
        {
            set { _queueDescription.RequiresDuplicateDetection = value; }
        }

        public QueueDescription QueueDescription
        {
            get { return _queueDescription; }
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (ValidationResult result in base.Validate())
                yield return result;

            if (_prefetchCount <= 0)
                yield return this.Failure("PrefetchCount", "must be > 0");
        }

        public void Apply(IBusBuilder builder)
        {
            ReceiveSettings settings = new ReceiveEndpointSettings
            {
                AutoRenewTimeout = TimeSpan.FromMinutes(5),
                MaxConcurrentCalls = _maxConcurrentCalls,
                PrefetchCount = _prefetchCount,
                QueueDescription = _queueDescription,
            };

            ServiceBusReceiveEndpointBuilder endpointBuilder = null;
            IPipe<ReceiveContext> receivePipe = CreateReceivePipe(builder, consumePipe =>
            {
                endpointBuilder = new ServiceBusReceiveEndpointBuilder(consumePipe, _host.MessageNameFormatter);
                return endpointBuilder;
            });

            if (endpointBuilder == null)
                throw new InvalidOperationException("The endpoint builder was not initialized");

            var transport = new ServiceBusReceiveTransport(_host, settings, endpointBuilder.GetTopicSubscriptions().ToArray());

            builder.AddReceiveEndpoint(new ReceiveEndpoint(transport, receivePipe));
        }

        public bool EnableExpress
        {
            set { _queueDescription.EnableExpress = value; }
        }

        public TimeSpan LockDuration
        {
            set { _queueDescription.LockDuration = value; }
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set { _queueDescription.EnableDeadLetteringOnMessageExpiration = value; }
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set { _queueDescription.DefaultMessageTimeToLive = value; }
        }

        public int PrefetchCount
        {
            set { _prefetchCount = value; }
        }

        public int MaxConcurrentCalls
        {
            set
            {
                _maxConcurrentCalls = value;
                if (_maxConcurrentCalls > _prefetchCount)
                    _prefetchCount = _maxConcurrentCalls;
            }
        }

        public Uri QueuePath
        {
            get { return _host.Settings.GetInputAddress(_queueDescription); }
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _queueDescription.RequiresDuplicateDetection = true;
            _queueDescription.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set { _queueDescription.AutoDeleteOnIdle = value; }
        }

        protected override Uri GetErrorAddress()
        {
            string errorQueueName = _queueDescription.Path + "_error";
            var errorQueueDescription = new QueueDescription(errorQueueName);

            return _host.Settings.GetInputAddress(errorQueueDescription);
        }


        class ReceiveEndpointSettings :
            ReceiveSettings
        {
            public int PrefetchCount { get; set; }
            public int MaxConcurrentCalls { get; set; }
            public QueueDescription QueueDescription { get; set; }
            public TimeSpan AutoRenewTimeout { get; set; }
        }
    }
}