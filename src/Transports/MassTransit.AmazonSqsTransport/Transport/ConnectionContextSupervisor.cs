namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using Pipeline;
    using Topology;
    using Topology.Builders;
    using Topology.Settings;
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

        public IClientContextSupervisor CreateClientContextSupervisor()
        {
            var clientContextSupervisor = new ClientContextSupervisor(this);

            AddConsumeAgent(clientContextSupervisor);

            return clientContextSupervisor;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreateSendTransport(IClientContextSupervisor supervisor, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            if (endpointAddress.Type == AmazonSqsEndpointAddress.AddressType.Queue)
            {
                var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

                IPipe<ClientContext> configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

                var transportContext = new SendTransportContext(_hostConfiguration, supervisor, configureTopology, settings.EntityName);

                var transport = new QueueSendTransport(transportContext);

                supervisor.AddSendAgent(transport);

                return Task.FromResult<ISendTransport>(transport);
            }
            else
            {
                var settings = new TopicPublishSettings(endpointAddress);

                var builder = new PublishEndpointBrokerTopologyBuilder();
                var topicHandle = builder.CreateTopic(settings.EntityName, settings.Durable, settings.AutoDelete, settings.TopicAttributes, settings
                    .TopicSubscriptionAttributes, settings.Tags);

                builder.Topic ??= topicHandle;

                IPipe<ClientContext> configureTopology = new ConfigureTopologyFilter<PublishSettings>(settings, builder.BuildBrokerTopology()).ToPipe();

                var transportContext = new SendTransportContext(_hostConfiguration, supervisor, configureTopology, settings.EntityName);

                var transport = new TopicSendTransport(transportContext);
                supervisor.AddSendAgent(transport);

                return Task.FromResult<ISendTransport>(transport);
            }
        }

        public Task<ISendTransport> CreatePublishTransport<T>(IClientContextSupervisor supervisor)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IAmazonSqsMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetPublishSettings(_hostConfiguration.HostAddress);

            IPipe<ClientContext> configureTopology = new ConfigureTopologyFilter<PublishSettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            var transportContext = new SendTransportContext(_hostConfiguration, supervisor, configureTopology, settings.EntityName);

            var transport = new TopicSendTransport(transportContext);
            supervisor.AddSendAgent(transport);

            return Task.FromResult<ISendTransport>(transport);
        }


        class SendTransportContext :
            BaseSendTransportContext,
            SqsSendTransportContext
        {
            public SendTransportContext(IAmazonSqsHostConfiguration hostConfiguration, IClientContextSupervisor clientContextSupervisor,
                IPipe<ClientContext> configureTopologyPipe, string entityName)
                : base(hostConfiguration)
            {
                ClientContextSupervisor = clientContextSupervisor;

                ConfigureTopologyPipe = configureTopologyPipe;
                EntityName = entityName;

                SqsSetHeaderAdapter = new TransportSetHeaderAdapter<MessageAttributeValue>(
                    new SqsHeaderValueConverter(hostConfiguration.Settings.AllowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
                SnsSetHeaderAdapter = new TransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue>(
                    new SnsHeaderValueConverter(hostConfiguration.Settings.AllowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
            }

            public IPipe<ClientContext> ConfigureTopologyPipe { get; }
            public string EntityName { get; }
            public IClientContextSupervisor ClientContextSupervisor { get; }
            public ITransportSetHeaderAdapter<MessageAttributeValue> SqsSetHeaderAdapter { get; }
            public ITransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue> SnsSetHeaderAdapter { get; }
        }
    }
}
