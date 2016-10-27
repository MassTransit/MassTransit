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
    using EndpointConfigurators;
    using GreenPipes;
    using GreenPipes.Specifications;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Pipeline;
    using Settings;
    using Transport;
    using Transports;


    public abstract class ServiceBusEndpointSpecification :
        ReceiveEndpointSpecification,
        IServiceBusEndpointConfigurator,
        IReceiveEndpointSpecification<IBusBuilder>
    {
        readonly BaseClientSettings _settings;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;

        protected ServiceBusEndpointSpecification(IServiceBusHost host, BaseClientSettings settings, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            Host = host;
            _settings = settings;
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider;

        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider;

        public IServiceBusHost Host { get; }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public abstract void Apply(IBusBuilder builder);

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

        public TimeSpan AutoDeleteOnIdle
        {
            set { _settings.AutoDeleteOnIdle = value; }
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set { _settings.DefaultMessageTimeToLive = value; }
        }

        public bool EnableBatchedOperations
        {
            set { _settings.EnableBatchedOperations = value; }
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set { _settings.EnableDeadLetteringOnMessageExpiration = value; }
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set { _settings.ForwardDeadLetteredMessagesTo = value; }
        }

        public TimeSpan LockDuration
        {
            set { _settings.LockDuration = value; }
        }

        public int MaxDeliveryCount
        {
            set { _settings.MaxDeliveryCount = value; }
        }

        public bool RequiresSession
        {
            set { _settings.RequiresSession = value; }
        }

        public string UserMetadata
        {
            set { _settings.UserMetadata = value; }
        }

        protected void ApplyReceiveEndpoint(IReceiveEndpointBuilder builder, IReceivePipe receivePipe, params IFilter<NamespaceContext>[] filters)
        {
            _sendEndpointProvider = CreateSendEndpointProvider(builder);
            _publishEndpointProvider = CreatePublishEndpointProvider(builder);

            IPipeSpecification<NamespaceContext>[] specifications = filters
                .Concat(Enumerable.Repeat(_settings.RequiresSession
                    ? (IFilter<NamespaceContext>)new MessageSessionReceiverFilter(receivePipe, _sendEndpointProvider, _publishEndpointProvider)
                    : new MessageReceiverFilter(receivePipe, _sendEndpointProvider, _publishEndpointProvider), 1))
                .Select(x => (IPipeSpecification<NamespaceContext>)new FilterPipeSpecification<NamespaceContext>(x))
                .ToArray();

            var transport = new ReceiveTransport(Host, _settings, specifications);

            var serviceBusHost = Host as ServiceBusHost;
            if (serviceBusHost == null)
                throw new ConfigurationException("Must be a ServiceBusHost");

            serviceBusHost.ReceiveEndpoints.Add(_settings.Path, new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetInputAddress()
        {
            return _settings.GetInputAddress(Host.Settings.ServiceUri);
        }

        protected override Uri GetErrorAddress()
        {
            var errorQueueName = _settings.Path + "_error";

            var endpointSettings = GetReceiveEndpointSettings(errorQueueName);

            return endpointSettings.GetInputAddress(Host.Settings.ServiceUri);
        }

        protected override Uri GetDeadLetterAddress()
        {
            var skippedQueueName = _settings.Path + "_skipped";

            var endpointSettings = GetReceiveEndpointSettings(skippedQueueName);

            return endpointSettings.GetInputAddress(Host.Settings.ServiceUri);
        }

        protected abstract ReceiveEndpointSettings GetReceiveEndpointSettings(string queueName);
    }
}