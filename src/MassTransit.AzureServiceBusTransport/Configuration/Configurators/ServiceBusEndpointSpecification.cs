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
    using EndpointSpecifications;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Pipeline;
    using Settings;
    using Specifications;
    using Topology;
    using Topology.Configuration;
    using Transport;
    using Transports;


    public abstract class ServiceBusEndpointSpecification :
        ReceiveEndpointSpecification,
        IServiceBusEndpointConfigurator,
        IReceiveEndpointSpecification<IBusBuilder>
    {
        readonly BaseClientSettings _settings;
        readonly IEndpointEntityConfigurator _configurator;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;

        protected ServiceBusEndpointSpecification(IServiceBusHost host, BaseClientSettings settings, IEndpointEntityConfigurator configurator, IServiceBusEndpointConfiguration configuration)
            : base(configuration)
        {
            Host = host;
            _settings = settings;
            _configurator = configurator;
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
            set => _settings.PrefetchCount = value; }

        public TimeSpan AutoDeleteOnIdle
        {
            set
            {
                _configurator.AutoDeleteOnIdle = value;
                
                Changed(nameof(AutoDeleteOnIdle));
            }
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set => _configurator.DefaultMessageTimeToLive = value; }

        public bool EnableBatchedOperations
        {
            set => _configurator.EnableBatchedOperations = value; }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set => _configurator.EnableDeadLetteringOnMessageExpiration = value; }

        public string ForwardDeadLetteredMessagesTo
        {
            set => _configurator.ForwardDeadLetteredMessagesTo = value; }

        public TimeSpan LockDuration
        {
            set => _configurator.LockDuration = value; }

        public int MaxDeliveryCount
        {
            set => _configurator.MaxDeliveryCount = value; }

        public bool RequiresSession
        {
            set => _configurator.RequiresSession = value; }

        public string UserMetadata
        {
            set => _configurator.UserMetadata = value; }

        public virtual void SelectBasicTier()
        {
            _settings.SelectBasicTier();
        }

        protected void ApplyReceiveEndpoint(IReceivePipe receivePipe, IServiceBusReceiveEndpointTopology receiveEndpointTopology,
            Action<IPipeConfigurator<NamespaceContext>> configurePipe)
        {
            _sendEndpointProvider = receiveEndpointTopology.SendEndpointProvider;
            _publishEndpointProvider = receiveEndpointTopology.PublishEndpointProvider;

            IPipe<NamespaceContext> pipe = Pipe.New<NamespaceContext>(x =>
            {
                configurePipe(x);

                if (_settings.RequiresSession)
                    x.UseFilter(new MessageSessionReceiverFilter(receivePipe, receiveEndpointTopology));
                else
                    x.UseFilter(new MessageReceiverFilter(receivePipe, receiveEndpointTopology));
            });

            var transport = new ReceiveTransport(Host, _settings, _publishEndpointProvider, _sendEndpointProvider, pipe);

            var serviceBusHost = Host as ServiceBusHost;
            if (serviceBusHost == null)
                throw new ConfigurationException("Must be a ServiceBusHost");

            serviceBusHost.ReceiveEndpoints.Add(_settings.Path, new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetInputAddress()
        {
            return _settings.GetInputAddress(Host.Settings.ServiceUri, _settings.Path);
        }

        protected override Uri GetErrorAddress()
        {
            var errorQueueName = _settings.Path + "_error";

            return _settings.GetInputAddress(Host.Settings.ServiceUri, errorQueueName);
        }

        protected override Uri GetDeadLetterAddress()
        {
            var skippedQueueName = _settings.Path + "_skipped";

            return _settings.GetInputAddress(Host.Settings.ServiceUri, skippedQueueName);
        }
    }
}