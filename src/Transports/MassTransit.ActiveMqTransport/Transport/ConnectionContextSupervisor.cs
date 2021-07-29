namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading;
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
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, address);

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

        Task<ISendTransport> CreateSendTransport(ISessionContextSupervisor sessionContextSupervisor, IPipe<SessionContext> pipe, string entityName,
            DestinationType destinationType)
        {
            var supervisor = new SessionContextSupervisor(sessionContextSupervisor);

            var sendTransportContext = new SendTransportContext(_hostConfiguration, supervisor, pipe, entityName, destinationType);

            var transport = new ActiveMqSendTransport(sendTransportContext);

            sessionContextSupervisor.AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }


        class SendTransportContext :
            BaseSendTransportContext,
            ActiveMqSendTransportContext
        {
            readonly IActiveMqHostConfiguration _hostConfiguration;

            public SendTransportContext(IActiveMqHostConfiguration hostConfiguration, ISessionContextSupervisor sessionContextSupervisor,
                IPipe<SessionContext> configureTopologyPipe, string entityName, DestinationType destinationType)
                : base(hostConfiguration)
            {
                _hostConfiguration = hostConfiguration;
                SessionContextSupervisor = sessionContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                EntityName = entityName;
                DestinationType = destinationType;
            }

            public IPipe<SessionContext> ConfigureTopologyPipe { get; }
            public string EntityName { get; }
            public DestinationType DestinationType { get; }
            public ISessionContextSupervisor SessionContextSupervisor { get; }

            public Task Send(IPipe<SessionContext> pipe, CancellationToken cancellationToken)
            {
                return _hostConfiguration.Retry(() => SessionContextSupervisor.Send(pipe, cancellationToken), SessionContextSupervisor, cancellationToken);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
