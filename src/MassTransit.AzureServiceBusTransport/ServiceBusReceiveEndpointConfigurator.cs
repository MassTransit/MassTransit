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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configuration;
    using Configurators;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using Microsoft.ServiceBus.Messaging;
    using PipeConfigurators;
    using Transports;


    public class ServiceBusReceiveEndpointConfigurator :
        IServiceBusReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IList<IReceiveEndpointSpecification> _configurators;
        readonly IConsumePipe _consumePipe;
        readonly IList<IPipeSpecification<ConsumeContext>> _consumePipeSpecifications;
        readonly IServiceBusHost _host;
        readonly QueueDescription _queueDescription;
        readonly IBuildPipeConfigurator<ReceiveContext> _receivePipeConfigurator;
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
        {
            _consumePipeSpecifications = new List<IPipeSpecification<ConsumeContext>>();
            _receivePipeConfigurator = new PipeConfigurator<ReceiveContext>();
            _configurators = new List<IReceiveEndpointSpecification>();

            _host = host;
            _queueDescription = queueDescription;
            _consumePipe = consumePipe;
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

        public IEnumerable<ValidationResult> Validate()
        {
            if (_prefetchCount <= 0)
                yield return this.Failure("PrefetchCount", "must be > 0");
        }

        public void Apply(IBusBuilder builder)
        {
            builder.AddReceiveEndpoint(CreateReceiveEndpoint(builder));
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecifications.Add(specification);
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _configurators.Add(configurator);
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

        ReceiveEndpoint CreateReceiveEndpoint(IBusBuilder builder)
        {
            ReceiveSettings settings = new ReceiveEndpointSettings
            {
                AutoRenewTimeout = TimeSpan.FromMinutes(5),
                MaxConcurrentCalls = _maxConcurrentCalls,
                PrefetchCount = _prefetchCount,
                QueueDescription = _queueDescription,
            };

            IConsumePipe consumePipe = _consumePipe ?? builder.CreateConsumePipe(_consumePipeSpecifications);

            var endpointBuilder = new ServiceBusReceiveEndpointBuilder(consumePipe, _host.MessageNameFormatter);

            foreach (IReceiveEndpointSpecification builderConfigurator in _configurators)
                builderConfigurator.Configure(endpointBuilder);

            string errorQueueName = _queueDescription.Path + "_error";
            var errorQueueDescription = new QueueDescription(errorQueueName);

            Uri errorAddress = _host.Settings.GetInputAddress(errorQueueDescription);

            ISendTransportProvider transportProvider = builder.SendTransportProvider;
            IPipe<ReceiveContext> moveToErrorPipe = Pipe.New<ReceiveContext>(
                x => x.Filter(new MoveToErrorTransportFilter(() => transportProvider.GetSendTransport(errorAddress))));

            _receivePipeConfigurator.Rescue(moveToErrorPipe, typeof(Exception));

            _receivePipeConfigurator.Filter(new DeserializeFilter(builder.MessageDeserializer, consumePipe));

            IPipe<ReceiveContext> receivePipe = _receivePipeConfigurator.Build();

            var transport = GetReceiveTransport(settings, endpointBuilder.GetTopicSubscriptions());

            return new ReceiveEndpoint(transport, receivePipe);
        }

        ServiceBusReceiveTransport GetReceiveTransport(ReceiveSettings settings, IEnumerable<TopicSubscriptionSettings> topicSubscriptionSettings)
        {
            return new ServiceBusReceiveTransport(_host, settings, topicSubscriptionSettings.ToArray());
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