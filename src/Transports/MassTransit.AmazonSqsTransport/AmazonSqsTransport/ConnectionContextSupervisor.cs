namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Topology;
    using Transports;


    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly IAmazonSqsTopologyConfiguration _topologyConfiguration;

        public ConnectionContextSupervisor(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsTopologyConfiguration topologyConfiguration)
            : base(new ConnectionContextFactory(hostConfiguration))
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreateSendTransport(SqsReceiveEndpointContext receiveEndpointContext, IClientContextSupervisor clientContextSupervisor,
            Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            if (endpointAddress.Type == AmazonSqsEndpointAddress.AddressType.Queue)
            {
                var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

                IPipe<ClientContext> configureTopology = new ConfigureAmazonSqsTopologyFilter<EntitySettings>(settings, settings.GetBrokerTopology()).ToPipe();

                return CreateTransport(receiveEndpointContext, clientContextSupervisor, configureTopology, settings.EntityName, x => new QueueSendTransport(x));
            }
            else
            {
                var settings = new TopicPublishSettings(endpointAddress);

                var builder = new PublishEndpointBrokerTopologyBuilder();
                var topicHandle = builder.CreateTopic(settings.EntityName, settings.Durable, settings.AutoDelete, settings.TopicAttributes, settings
                    .TopicSubscriptionAttributes, settings.Tags);

                builder.Topic ??= topicHandle;

                IPipe<ClientContext> configureTopology = new ConfigureAmazonSqsTopologyFilter<EntitySettings>(settings, builder.BuildBrokerTopology()).ToPipe();

                return CreateTransport(receiveEndpointContext, clientContextSupervisor, configureTopology, settings.EntityName, x => new TopicSendTransport(x));
            }
        }

        public Task<ISendTransport> CreatePublishTransport<T>(SqsReceiveEndpointContext receiveEndpointContext,
            IClientContextSupervisor clientContextSupervisor)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IAmazonSqsMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetPublishSettings(_hostConfiguration.HostAddress);

            IPipe<ClientContext> configureTopology =
                new ConfigureAmazonSqsTopologyFilter<EntitySettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            return CreateTransport(receiveEndpointContext, clientContextSupervisor, configureTopology, settings.EntityName, x => new TopicSendTransport(x));
        }

        Task<ISendTransport> CreateTransport<T>(SqsReceiveEndpointContext receiveEndpointContext, IClientContextSupervisor clientContextSupervisor,
            IPipe<ClientContext> configureTopology, string entityName, Func<SqsSendTransportContext, T> factory)
            where T : Supervisor, ISendTransport
        {
            var supervisor = new ClientContextSupervisor(clientContextSupervisor);

            var transportContext = new SendTransportContext(_hostConfiguration, receiveEndpointContext, supervisor, configureTopology, entityName);

            var transport = factory(transportContext);

            clientContextSupervisor.AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }


        class SendTransportContext :
            BaseSendTransportContext,
            SqsSendTransportContext
        {
            readonly IAmazonSqsHostConfiguration _hostConfiguration;

            public SendTransportContext(IAmazonSqsHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext,
                IClientContextSupervisor clientContextSupervisor, IPipe<ClientContext> configureTopologyPipe, string entityName)
                : base(hostConfiguration, receiveEndpointContext.Serialization)
            {
                _hostConfiguration = hostConfiguration;
                ClientContextSupervisor = clientContextSupervisor;

                ConfigureTopologyPipe = configureTopologyPipe;
                EntityName = entityName;

                SqsSetHeaderAdapter = new TransportSetHeaderAdapter<MessageAttributeValue>(
                    new SqsHeaderValueConverter(hostConfiguration.Settings.AllowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
                SnsSetHeaderAdapter = new TransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue>(
                    new SnsHeaderValueConverter(hostConfiguration.Settings.AllowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
            }

            public IPipe<ClientContext> ConfigureTopologyPipe { get; }
            public override string EntityName { get; }
            public override string ActivitySystem => "sqs";
            public IClientContextSupervisor ClientContextSupervisor { get; }
            public ITransportSetHeaderAdapter<MessageAttributeValue> SqsSetHeaderAdapter { get; }
            public ITransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue> SnsSetHeaderAdapter { get; }

            public Task Send(IPipe<ClientContext> pipe, CancellationToken cancellationToken)
            {
                return _hostConfiguration.Retry(() => ClientContextSupervisor.Send(pipe, cancellationToken), ClientContextSupervisor, cancellationToken);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
