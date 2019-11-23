namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Configuration.Configuration;
    using Configurators;
    using Context;
    using Contexts;
    using Definition;
    using Exceptions;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Transports;


    public class AmazonSqsHost :
        BaseHost,
        IAmazonSqsHostControl
    {
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly IAmazonSqsHostTopology _hostTopology;

        public AmazonSqsHost(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            _hostTopology = hostTopology;

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<AmazonSqsTransportException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionContextSupervisor = new AmazonSqsConnectionContextSupervisor(hostConfiguration, hostTopology);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public AmazonSqsHostSettings Settings => _hostConfiguration.Settings;
        public IAmazonSqsHostTopology Topology => _hostTopology;

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "AmazonSQS",
                Settings.Region,
                Settings.AccessKey,
                Password = new string('*', Settings.SecretKey.Length)
            });

            ConnectionContextSupervisor.Probe(context);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IAmazonSqsReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            LogContext.Debug?.Log("Connect receive endpoint: {Queue}", queueName);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build(this);

            return ReceiveEndpoints.Start(queueName);
        }

        public Task<ISendTransport> CreateSendTransport(AmazonSqsEndpointAddress address)
        {
            var settings = _hostTopology.SendTopology.GetSendSettings(address);

            var clientContextSupervisor = new AmazonSqsClientContextSupervisor(ConnectionContextSupervisor);

            var configureTopologyPipe = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

            var transportContext = new HostSqsSendTransportContext(clientContextSupervisor, configureTopologyPipe, settings.EntityName, SendLogContext);

            var transport = new QueueSendTransport(transportContext);
            Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IAmazonSqsMessagePublishTopology<T> publishTopology = _hostTopology.Publish<T>();

            var settings = publishTopology.GetPublishSettings();

            var clientContextSupervisor = new AmazonSqsClientContextSupervisor(ConnectionContextSupervisor);

            var configureTopologyPipe = new ConfigureTopologyFilter<PublishSettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            var transportContext = new HostSqsSendTransportContext(clientContextSupervisor, configureTopologyPipe, settings.EntityName, SendLogContext);

            var transport = new TopicSendTransport(transportContext);
            Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {ConnectionContextSupervisor};
        }
    }
}
