namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Middleware;
    using Topology;
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

        public Task<ISendTransport> CreateSendTransport(RabbitMqReceiveEndpointContext receiveEndpointContext,
            IModelContextSupervisor modelContextSupervisor, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new RabbitMqEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            var brokerTopology = settings.GetBrokerTopology();

            IPipe<ModelContext> configureTopology = new ConfigureRabbitMqTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

            return CreateSendTransport(receiveEndpointContext, modelContextSupervisor, configureTopology, settings.ExchangeName, endpointAddress);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(RabbitMqReceiveEndpointContext receiveEndpointContext,
            IModelContextSupervisor modelContextSupervisor)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IRabbitMqMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            var brokerTopology = publishTopology.GetBrokerTopology();

            IPipe<ModelContext> configureTopology = new ConfigureRabbitMqTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

            var endpointAddress = settings.GetSendAddress(_hostConfiguration.HostAddress);

            return CreateSendTransport(receiveEndpointContext, modelContextSupervisor, configureTopology, publishTopology.Exchange.ExchangeName,
                endpointAddress);
        }

        Task<ISendTransport> CreateSendTransport(RabbitMqReceiveEndpointContext receiveEndpointContext,
            IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> pipe, string exchangeName, RabbitMqEndpointAddress endpointAddress)
        {
            var supervisor = new ModelContextSupervisor(modelContextSupervisor);

            var delayedExchangeAddress = endpointAddress.GetDelayAddress();

            var delaySettings = new RabbitMqDelaySettings(delayedExchangeAddress);

            delaySettings.BindToExchange(exchangeName);

            IPipe<ModelContext> delayPipe = new ConfigureRabbitMqTopologyFilter<DelaySettings>(delaySettings, delaySettings.GetBrokerTopology()).ToPipe();

            var sendTransportContext = new SendTransportContext(_hostConfiguration, receiveEndpointContext, supervisor, pipe, exchangeName, delayPipe,
                delaySettings.ExchangeName);

            var transport = new RabbitMqSendTransport(sendTransportContext);

            modelContextSupervisor.AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }


        class SendTransportContext :
            BaseSendTransportContext,
            RabbitMqSendTransportContext
        {
            readonly IRabbitMqHostConfiguration _hostConfiguration;

            public SendTransportContext(IRabbitMqHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
                IModelContextSupervisor modelContextSupervisor,
                IPipe<ModelContext> configureTopologyPipe, string exchange,
                IPipe<ModelContext> delayConfigureTopologyPipe, string delayExchange)
                : base(hostConfiguration, receiveEndpointContext.Serialization)
            {
                _hostConfiguration = hostConfiguration;
                ModelContextSupervisor = modelContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                Exchange = exchange;

                DelayConfigureTopologyPipe = delayConfigureTopologyPipe;
                DelayExchange = delayExchange;
            }

            public IPipe<ModelContext> ConfigureTopologyPipe { get; }
            public IPipe<ModelContext> DelayConfigureTopologyPipe { get; }
            public string DelayExchange { get; }
            public string Exchange { get; }
            public IModelContextSupervisor ModelContextSupervisor { get; }

            public Task Send(IPipe<ModelContext> pipe, CancellationToken cancellationToken)
            {
                return _hostConfiguration.Retry(() => ModelContextSupervisor.Send(pipe, cancellationToken), ModelContextSupervisor, cancellationToken);
            }

            public void Probe(ProbeContext context)
            {
            }

            public override string EntityName => Exchange;
            public override string ActivitySystem => "rabbitmq";
        }
    }
}
