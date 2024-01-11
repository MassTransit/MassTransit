namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configuration;
    using Middleware;
    using Transports;


    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly IActiveMqTopologyConfiguration _topologyConfiguration;

        public ConnectionContextSupervisor(IActiveMqHostConfiguration hostConfiguration, IActiveMqTopologyConfiguration topologyConfiguration)
            : base(new ConnectionContextFactory(hostConfiguration))
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreateSendTransport(ActiveMqReceiveEndpointContext context, ISessionContextSupervisor sessionContextSupervisor, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            IPipe<SessionContext> configureTopology = new ConfigureActiveMqTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology(), context)
                .ToPipe();

            return CreateSendTransport(context, sessionContextSupervisor, configureTopology, settings.EntityName,
                endpointAddress.Type == ActiveMqEndpointAddress.AddressType.Queue ? DestinationType.Queue : DestinationType.Topic);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(ActiveMqReceiveEndpointContext context, ISessionContextSupervisor sessionContextSupervisor)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IActiveMqMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            IPipe<SessionContext> configureTopology = new ConfigureActiveMqTopologyFilter<SendSettings>(settings, publishTopology.GetBrokerTopology(), context)
                .ToPipe();

            return CreateSendTransport(context, sessionContextSupervisor, configureTopology, settings.EntityName, DestinationType.Topic);
        }

        Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext context, ISessionContextSupervisor sessionContextSupervisor,
            IPipe<SessionContext> pipe, string entityName, DestinationType destinationType)
        {
            var supervisor = new SessionContextSupervisor(sessionContextSupervisor);

            var sendTransportContext = new ActiveMqSendTransportContext(_hostConfiguration, context, supervisor, pipe, entityName, destinationType);

            var transport = new SendTransport<SessionContext>(sendTransportContext);

            sessionContextSupervisor.AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }
    }
}
