namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;
    using Transport;
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

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IServiceBusMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings();

            var transport = CreateSendTransport(publishAddress, settings);

            return Task.FromResult(transport);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, address);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            var transport = CreateSendTransport(endpointAddress, settings);

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

            var configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology(), false, Stopping);

            var contextFactory = new SendEndpointContextFactory(this, configureTopology.ToPipe<SendEndpointContext>(), settings);

            var contextSupervisor = new SendEndpointContextSupervisor(contextFactory);

            AddSendAgent(contextSupervisor);

            return contextSupervisor;
        }

        ISendTransport CreateSendTransport(Uri address, SendSettings settings)
        {
            TransportLogMessages.CreateSendTransport(address);

            var endpointContextSupervisor = CreateSendEndpointContextSupervisor(settings);

            var transportContext = new SendTransportContext(_hostConfiguration, address, endpointContextSupervisor);

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

            public SendTransportContext(IServiceBusHostConfiguration hostConfiguration, Uri address, ISendEndpointContextSupervisor supervisor)
                : base(hostConfiguration)
            {
                _hostConfiguration = hostConfiguration;
                Address = address;
                _supervisor = supervisor;
            }

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
