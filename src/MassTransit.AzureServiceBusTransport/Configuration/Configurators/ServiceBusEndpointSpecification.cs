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
    using System.Collections.Generic;
    using Contexts;
    using EndpointSpecifications;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Logging;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
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
        readonly IEndpointEntityConfigurator _configurator;
        readonly ServiceBusHost _host;
        readonly BaseClientSettings _settings;
        protected readonly IBuildPipeConfigurator<ClientContext> ClientPipeConfigurator;
        protected readonly IBuildPipeConfigurator<MessagingFactoryContext> MessagingFactoryPipeConfigurator;
        protected readonly IBuildPipeConfigurator<NamespaceContext> NamespacePipeConfigurator;
        IPublishEndpointProvider _publishEndpointProvider;
        ISendEndpointProvider _sendEndpointProvider;

        protected ServiceBusEndpointSpecification(ServiceBusHost host, BaseClientSettings settings, IEndpointEntityConfigurator configurator,
            IServiceBusEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
            _settings = settings;
            _configurator = configurator;

            ClientPipeConfigurator = new PipeConfigurator<ClientContext>();
            NamespacePipeConfigurator = new PipeConfigurator<NamespaceContext>();
            MessagingFactoryPipeConfigurator = new PipeConfigurator<MessagingFactoryContext>();
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider;

        public IServiceBusHost Host => _host;

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
            set => _settings.PrefetchCount = value;
        }

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
            set => _configurator.DefaultMessageTimeToLive = value;
        }

        public bool EnableBatchedOperations
        {
            set => _configurator.EnableBatchedOperations = value;
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set => _configurator.EnableDeadLetteringOnMessageExpiration = value;
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set => _configurator.ForwardDeadLetteredMessagesTo = value;
        }

        public TimeSpan LockDuration
        {
            set => _configurator.LockDuration = value;
        }

        public int MaxDeliveryCount
        {
            set => _configurator.MaxDeliveryCount = value;
        }

        public bool RequiresSession
        {
            set => _configurator.RequiresSession = value;
        }

        public string UserMetadata
        {
            set => _configurator.UserMetadata = value;
        }

        public virtual void SelectBasicTier()
        {
            _settings.SelectBasicTier();
        }

        protected void ApplyReceiveEndpoint(IReceivePipe receivePipe, IServiceBusReceiveEndpointTopology receiveEndpointTopology)
        {
            _sendEndpointProvider = receiveEndpointTopology.SendEndpointProvider;
            _publishEndpointProvider = receiveEndpointTopology.PublishEndpointProvider;

            var messageReceiver = new BrokeredMessageReceiver(InputAddress, receivePipe, Logger.Get<Receiver>(), receiveEndpointTopology);

            var transportObserver = new ReceiveTransportObservable();

            var clientCache = CreateClientCache(InputAddress, _host.MessagingFactoryCache, _host.NamespaceCache);

            var errorTransport = CreateErrorTransport(_host);
            var deadLetterTransport = CreateDeadLetterTransport(_host);

            var receiverFilter = _settings.RequiresSession
                ? new MessageSessionReceiverFilter(messageReceiver, transportObserver, deadLetterTransport, errorTransport)
                : new MessageReceiverFilter(messageReceiver, transportObserver, deadLetterTransport, errorTransport);

            ClientPipeConfigurator.UseFilter(receiverFilter);


            IPipe<ClientContext> clientPipe = ClientPipeConfigurator.Build();

            var transport = new ReceiveTransport(_host, _settings, _publishEndpointProvider, _sendEndpointProvider, clientCache, clientPipe, transportObserver);

            transport.Add(receiverFilter);

            _host.ReceiveEndpoints.Add(_settings.Name, new ReceiveEndpoint(transport, receivePipe));
        }

        protected abstract IErrorTransport CreateErrorTransport(ServiceBusHost host);
        protected abstract IDeadLetterTransport CreateDeadLetterTransport(ServiceBusHost host);

        protected override Uri GetInputAddress()
        {
            return _settings.GetInputAddress(_host.Settings.ServiceUri, _settings.Path);
        }

        protected abstract IClientCache CreateClientCache(Uri inputAddress, IMessagingFactoryCache messagingFactoryCache, INamespaceCache namespaceCache);

        protected abstract IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings,
            IPipe<NamespaceContext> namespacePipe);

        protected ISource<SendEndpointContext> CreateSendEndpointContextCache(ServiceBusHost host, SendSettings settings)
        {
            var brokerTopology = settings.GetBrokerTopology();

            IPipe<NamespaceContext> namespacePipe = Pipe.New<NamespaceContext>(x => x.UseFilter(new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology, false)));

            IPipeContextFactory<SendEndpointContext> factory = CreateSendEndpointContextFactory(host, settings, namespacePipe);

            var cache = new SendEndpointContextCache(factory);
            host.Add(cache);

            return cache;
        }
    }
}