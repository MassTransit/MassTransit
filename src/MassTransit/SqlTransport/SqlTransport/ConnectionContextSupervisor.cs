#nullable enable
namespace MassTransit.SqlTransport
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
        readonly ISqlHostConfiguration _hostConfiguration;
        readonly ISqlTopologyConfiguration _topologyConfiguration;

        public ConnectionContextSupervisor(ISqlHostConfiguration hostConfiguration, ISqlTopologyConfiguration topologyConfiguration,
            IPipeContextFactory<ConnectionContext> connectionContextFactory)
            : base(connectionContextFactory)
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new SqlEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(SqlReceiveEndpointContext context, Uri? publishAddress)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            ISqlMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            var brokerTopology = publishTopology.GetBrokerTopology();

            IPipe<ClientContext> configureTopology = new ConfigureSqlTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

            var supervisor = new ClientContextSupervisor(context.ClientContextSupervisor);

            return CreateSendTransport(publishAddress!,
                new TopicSendTransportContext(_hostConfiguration, context, supervisor, configureTopology, settings.EntityName));
        }

        public Task<ISendTransport> CreateSendTransport(SqlReceiveEndpointContext context, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new SqlEndpointAddress(_hostConfiguration.HostAddress, address);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            IPipe<ClientContext> configureTopology = new ConfigureSqlTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

            var supervisor = new ClientContextSupervisor(context.ClientContextSupervisor);

            return CreateSendTransport(endpointAddress, endpointAddress.Type == SqlEndpointAddress.AddressType.Queue
                ? new QueueSendTransportContext(_hostConfiguration, context, supervisor, configureTopology, settings.EntityName)
                : new TopicSendTransportContext(_hostConfiguration, context, supervisor, configureTopology, settings.EntityName));
        }

        Task<ISendTransport> CreateSendTransport(Uri address, SendTransportContext<ClientContext> transportContext)
        {
            TransportLogMessages.CreateSendTransport(address);

            var transport = new SendTransport<ClientContext>(transportContext);

            AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }
    }
}
