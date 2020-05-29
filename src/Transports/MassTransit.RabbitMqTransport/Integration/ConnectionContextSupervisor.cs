namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class ConnectionContextSupervisor :
        PipeContextSupervisor<ConnectionContext>,
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

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new RabbitMqEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreateSendTransport(IModelContextSupervisor modelContextSupervisor, Uri address)
        {
            var endpointAddress = new RabbitMqEndpointAddress(_hostConfiguration.HostAddress, address);

            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

            var brokerTopology = settings.GetBrokerTopology();

            IPipe<ModelContext> pipe = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

            var supervisor = new ModelContextSupervisor(modelContextSupervisor);

            var transport = CreateSendTransport(supervisor, pipe, settings.ExchangeName);

            return Task.FromResult(transport);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(IModelContextSupervisor modelContextSupervisor)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IRabbitMqMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var sendSettings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            var brokerTopology = publishTopology.GetBrokerTopology();

            var supervisor = new ModelContextSupervisor(modelContextSupervisor);

            IPipe<ModelContext> pipe = new ConfigureTopologyFilter<SendSettings>(sendSettings, brokerTopology).ToPipe();

            var transport = CreateSendTransport(supervisor, pipe, publishTopology.Exchange.ExchangeName);

            return Task.FromResult(transport);
        }

        ISendTransport CreateSendTransport(IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> pipe, string exchangeName)
        {
            var sendTransportContext = new SendTransportContext(modelContextSupervisor, pipe, exchangeName, _hostConfiguration.SendLogContext);

            var transport = new RabbitMqSendTransport(sendTransportContext);
            Add(transport);

            return transport;
        }


        class SendTransportContext :
            BaseSendTransportContext,
            RabbitMqSendTransportContext
        {
            public SendTransportContext(IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> configureTopologyPipe, string exchange,
                ILogContext logContext)
                : base(logContext)
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
