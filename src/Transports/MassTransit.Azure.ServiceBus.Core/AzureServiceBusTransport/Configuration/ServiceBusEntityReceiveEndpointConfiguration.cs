namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Topology;
    using Transports;
    using Util;


    public abstract class ServiceBusEntityReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration
    {
        readonly IServiceBusEndpointEntityConfigurator _configurator;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly BaseClientSettings _settings;
        protected readonly IBuildPipeConfigurator<ClientContext> ClientPipeConfigurator;

        protected ServiceBusEntityReceiveEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration, BaseClientSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;
            _configurator = settings.Configurator;

            ClientPipeConfigurator = new PipeConfigurator<ClientContext>();
        }

        public int MaxConcurrentCalls
        {
            set => ConcurrentMessageLimit = value;
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

        public int MaxConcurrentCallsPerSession
        {
            set => _configurator.MaxConcurrentCallsPerSession = value;
        }

        public string UserMetadata
        {
            set => _configurator.UserMetadata = value;
        }

        public TimeSpan MessageWaitTimeout
        {
            set => _settings.SessionIdleTimeout = value;
        }

        public TimeSpan SessionIdleTimeout
        {
            set => _settings.SessionIdleTimeout = value;
        }

        public TimeSpan MaxAutoRenewDuration
        {
            set => _settings.MaxAutoRenewDuration = value;
        }

        public virtual void SelectBasicTier()
        {
            _settings.SelectBasicTier();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return ClientPipeConfigurator.Validate()
                .Concat(ValidateSettings())
                .Concat(base.Validate());
        }

        IEnumerable<ValidationResult> ValidateSettings()
        {
            if (_settings.PrefetchCount < 0)
                yield return this.Failure("PrefetchCount", "must be >= 0");

            if (_settings.MaxConcurrentCalls <= 0)
                yield return this.Failure("MaxConcurrentCalls", "must be > 0");
        }

        protected void CreateReceiveEndpoint(IHost host, ServiceBusReceiveEndpointContext receiveEndpointContext)
        {
            if (_hostConfiguration.DeployTopologyOnly)
                ClientPipeConfigurator.UseFilter(new TransportReadyFilter<ClientContext>(receiveEndpointContext));
            else
            {
                var messageReceiver = new ServiceBusMessageReceiver(receiveEndpointContext);

                ClientPipeConfigurator.UseFilter(_settings.RequiresSession
                    ? new MessageSessionReceiverFilter(messageReceiver, receiveEndpointContext)
                    : new MessageReceiverFilter(messageReceiver, receiveEndpointContext));
            }

            IPipe<ClientContext> clientPipe = ClientPipeConfigurator.Build();

            var transport = new ReceiveTransport<ClientContext>(_hostConfiguration, receiveEndpointContext, () => receiveEndpointContext
                .ClientContextSupervisor, clientPipe);

            var receiveEndpoint = new ReceiveEndpoint(transport, receiveEndpointContext);

            var queueName = _settings.Path ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }
    }
}
