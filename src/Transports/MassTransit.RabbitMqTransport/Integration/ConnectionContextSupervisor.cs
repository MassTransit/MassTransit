namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly IRabbitMqTopologyConfiguration _topologyConfiguration;

        public ConnectionContextSupervisor(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(new ConnectionContextFactory(hostConfiguration))
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new RabbitMqEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreateSendTransport(IModelContextSupervisor modelContextSupervisor, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new RabbitMqEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            var brokerTopology = settings.GetBrokerTopology();

            IPipe<ModelContext> configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

            return CreateSendTransport(modelContextSupervisor, configureTopology, settings.ExchangeName);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(IModelContextSupervisor modelContextSupervisor)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IRabbitMqMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            var brokerTopology = publishTopology.GetBrokerTopology();

            IPipe<ModelContext> configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

            return CreateSendTransport(modelContextSupervisor, configureTopology, publishTopology.Exchange.ExchangeName);
        }

        Task<ISendTransport> CreateSendTransport(IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> pipe, string exchangeName)
        {
            var supervisor = new ModelContextSupervisor(modelContextSupervisor);

            var sendTransportContext = new SendTransportContext(_hostConfiguration, supervisor, pipe, exchangeName);

            var transport = new RabbitMqSendTransport(sendTransportContext);

            modelContextSupervisor.AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }


        class SendTransportContext :
            BaseSendTransportContext,
            RabbitMqSendTransportContext
        {
            public SendTransportContext(IRabbitMqHostConfiguration hostConfiguration, IModelContextSupervisor modelContextSupervisor,
                IPipe<ModelContext> configureTopologyPipe, string exchange)
                : base(hostConfiguration)
            {
                ModelContextSupervisor = modelContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                Exchange = exchange;
            }

            public IPipe<ModelContext> ConfigureTopologyPipe { get; }
            public string Exchange { get; }
            public IModelContextSupervisor ModelContextSupervisor { get; }
        }
    }
}
