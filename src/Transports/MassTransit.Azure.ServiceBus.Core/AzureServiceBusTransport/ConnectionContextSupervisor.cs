namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Configuration;
    using Middleware;
    using Transports;


    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly IServiceBusTopologyConfiguration _topologyConfiguration;

        public ConnectionContextSupervisor(IServiceBusHostConfiguration hostConfiguration, IServiceBusTopologyConfiguration topologyConfiguration)
            : base(new ConnectionContextFactory(hostConfiguration))
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext receiveEndpointContext, Uri publishAddress)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IServiceBusMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings();

            var transport = CreateSendTransport(publishAddress, settings, receiveEndpointContext);

            return Task.FromResult(transport);
        }

        public Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext receiveEndpointContext, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, address);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            var transport = CreateSendTransport(endpointAddress, settings, receiveEndpointContext);

            return Task.FromResult(transport);
        }

        public IClientContextSupervisor CreateClientContextSupervisor(Func<IConnectionContextSupervisor, IPipeContextFactory<ClientContext>> factory)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var clientContextSupervisor = new ClientContextSupervisor(factory(this));

            AddConsumeAgent(clientContextSupervisor);

            return clientContextSupervisor;
        }

        public ISendEndpointContextSupervisor CreateSendEndpointContextSupervisor(SendSettings settings)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var configureTopology = new ConfigureServiceBusTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology(), false);

            var contextFactory = new SendEndpointContextFactory(this, configureTopology.ToPipe<SendEndpointContext>(), settings);

            var contextSupervisor = new SendEndpointContextSupervisor(contextFactory);

            AddSendAgent(contextSupervisor);

            return contextSupervisor;
        }

        ISendTransport CreateSendTransport(Uri address, SendSettings settings, ReceiveEndpointContext receiveEndpointContext)
        {
            TransportLogMessages.CreateSendTransport(address);

            var endpointContextSupervisor = CreateSendEndpointContextSupervisor(settings);

            var transportContext = new SendTransportContext(_hostConfiguration, receiveEndpointContext, address, endpointContextSupervisor, settings);

            var transport = new ServiceBusSendTransport(transportContext);

            endpointContextSupervisor.Add(transport);

            return transport;
        }


        class SendTransportContext :
            BaseSendTransportContext,
            ServiceBusSendTransportContext
        {
            readonly IServiceBusHostConfiguration _hostConfiguration;
            readonly ISendEndpointContextSupervisor _supervisor;

            public SendTransportContext(IServiceBusHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext, Uri address,
                ISendEndpointContextSupervisor supervisor, SendSettings settings)
                : base(hostConfiguration, receiveEndpointContext.Serialization)
            {
                _hostConfiguration = hostConfiguration;
                _supervisor = supervisor;

                Address = address;
                EntityName = settings.EntityPath;
            }

            public override string EntityName { get; }
            public override string ActivitySystem => "service-bus";

            public Uri Address { get; }

            public Task Send(IPipe<SendEndpointContext> pipe, CancellationToken cancellationToken)
            {
                return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), _supervisor, cancellationToken);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
