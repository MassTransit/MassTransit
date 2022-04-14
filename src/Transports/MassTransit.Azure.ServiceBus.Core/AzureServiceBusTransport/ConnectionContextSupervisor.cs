namespace MassTransit.AzureServiceBusTransport
{
    using System;
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

            return CreateSendTransport(publishAddress, settings, receiveEndpointContext);
        }

        public Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext receiveEndpointContext, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, address);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            return CreateSendTransport(endpointAddress, settings, receiveEndpointContext);
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

            return new SendEndpointContextSupervisor(contextFactory);
        }

        Task<ISendTransport> CreateSendTransport(Uri address, SendSettings settings, ReceiveEndpointContext receiveEndpointContext)
        {
            TransportLogMessages.CreateSendTransport(address);

            var supervisor = CreateSendEndpointContextSupervisor(settings);

            var transportContext = new ServiceBusSendTransportContext(_hostConfiguration, receiveEndpointContext, supervisor, settings);

            var transport = new SendTransport<SendEndpointContext>(transportContext);

            AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }
    }
}
