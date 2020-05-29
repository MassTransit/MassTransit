namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Topology.Builders;
    using Topology.Settings;
    using Transports;


    public class ConnectionContextSupervisor :
        PipeContextSupervisor<ConnectionContext>,
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

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            var endpointAddress = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);

            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            TransportLogMessages.CreateSendTransport(endpointAddress);

            if (endpointAddress.Type == AmazonSqsEndpointAddress.AddressType.Queue)
            {
                var settings = _topologyConfiguration.Send.GetSendSettings(endpointAddress);

                var clientContextSupervisor = new ClientContextSupervisor(this);

                IPipe<ClientContext> configureTopologyPipe = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

                var transportContext = new SendTransportContext(clientContextSupervisor, configureTopologyPipe, settings.EntityName,
                    _hostConfiguration.SendLogContext, _hostConfiguration.Settings.AllowTransportHeader);

                var transport = new QueueSendTransport(transportContext);
                Add(transport);

                return Task.FromResult<ISendTransport>(transport);
            }
            else
            {
                var settings = new TopicPublishSettings(endpointAddress);

                var clientContextSupervisor = new ClientContextSupervisor(this);

                var builder = new PublishEndpointBrokerTopologyBuilder();
                var topicHandle = builder.CreateTopic(settings.EntityName, settings.Durable, settings.AutoDelete, settings.TopicAttributes, settings
                    .TopicSubscriptionAttributes, settings.Tags);

                builder.Topic ??= topicHandle;

                IPipe<ClientContext> configureTopologyPipe = new ConfigureTopologyFilter<PublishSettings>(settings, builder.BuildBrokerTopology()).ToPipe();

                var transportContext = new SendTransportContext(clientContextSupervisor, configureTopologyPipe, settings.EntityName,
                    _hostConfiguration.SendLogContext, _hostConfiguration.Settings.AllowTransportHeader);

                var transport = new TopicSendTransport(transportContext);
                Add(transport);

                return Task.FromResult<ISendTransport>(transport);
            }
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            IAmazonSqsMessagePublishTopology<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            var settings = publishTopology.GetPublishSettings(_hostConfiguration.HostAddress);

            var clientContextSupervisor = new ClientContextSupervisor(this);

            IPipe<ClientContext> configureTopologyPipe = new ConfigureTopologyFilter<PublishSettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            var transportContext = new SendTransportContext(clientContextSupervisor, configureTopologyPipe, settings.EntityName,
                _hostConfiguration.SendLogContext, _hostConfiguration.Settings.AllowTransportHeader);

            var transport = new TopicSendTransport(transportContext);
            Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }


        class SendTransportContext :
            BaseSendTransportContext,
            SqsSendTransportContext
        {
            public SendTransportContext(IClientContextSupervisor clientContextSupervisor, IPipe<ClientContext> configureTopologyPipe, string entityName,
                ILogContext logContext, AllowTransportHeader allowTransportHeader)
                : base(logContext)
            {
                ClientContextSupervisor = clientContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                EntityName = entityName;

                SqsSetHeaderAdapter = new TransportSetHeaderAdapter<MessageAttributeValue>(new SqsHeaderValueConverter(allowTransportHeader),
                    TransportHeaderOptions.IncludeFaultMessage);
                SnsSetHeaderAdapter = new TransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue>(
                    new SnsHeaderValueConverter(allowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);
            }

            public IPipe<ClientContext> ConfigureTopologyPipe { get; }
            public string EntityName { get; }
            public IClientContextSupervisor ClientContextSupervisor { get; }

            public ITransportSetHeaderAdapter<MessageAttributeValue> SqsSetHeaderAdapter { get; }
            public ITransportSetHeaderAdapter<Amazon.SimpleNotificationService.Model.MessageAttributeValue> SnsSetHeaderAdapter { get; }
        }
    }
}
