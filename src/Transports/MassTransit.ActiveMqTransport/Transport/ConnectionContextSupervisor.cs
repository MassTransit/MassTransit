namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using Pipeline;
    using Topology;
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

        public Task<ISendTransport> CreateSendTransport(ISessionContextSupervisor sessionContextSupervisor, Uri address)
        {
            var endpointAddress = new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, address);

            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            IPipe<SessionContext> configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

            return CreateSendTransport(sessionContextSupervisor, configureTopology, settings.EntityName,
                endpointAddress.Type == ActiveMqEndpointAddress.AddressType.Queue ? DestinationType.Queue : DestinationType.Topic);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(ISessionContextSupervisor sessionContextSupervisor)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IActiveMqMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            IPipe<SessionContext> configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            return CreateSendTransport(sessionContextSupervisor, configureTopology, settings.EntityName, DestinationType.Topic);
        }

        public Task<ISendTransport> CreateSendTransport(ISessionContextSupervisor supervisor, IPipe<SessionContext> pipe, string entityName,
            DestinationType destinationType)
        {
            var sendTransportContext = new SendTransportContext(supervisor, pipe, entityName, destinationType, _hostConfiguration.SendLogContext);

            var transport = new ActiveMqSendTransport(sendTransportContext);

            AddAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }


        class SendTransportContext :
            BaseSendTransportContext,
            ActiveMqSendTransportContext
        {
            public SendTransportContext(ISessionContextSupervisor sessionContextSupervisor, IPipe<SessionContext> configureTopologyPipe, string
                entityName, DestinationType destinationType, ILogContext logContext)
                : base(logContext)
            {
                SessionContextSupervisor = sessionContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                EntityName = entityName;
                DestinationType = destinationType;
            }

            public IPipe<SessionContext> ConfigureTopologyPipe { get; }
            public string EntityName { get; }
            public DestinationType DestinationType { get; }
            public ISessionContextSupervisor SessionContextSupervisor { get; }
        }
    }
}
