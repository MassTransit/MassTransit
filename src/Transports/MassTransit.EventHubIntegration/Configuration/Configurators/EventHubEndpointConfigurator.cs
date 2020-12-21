namespace MassTransit.EventHubIntegration.Configurators
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Configuration;
    using Contexts;
    using Filters;
    using GreenPipes;
    using GreenPipes.Configurators;
    using MassTransit.Registration;
    using Transports;


    public class EventHubReceiveEndpointConfigurator :
        ReceiverConfiguration,
        IEventHubReceiveEndpointConfigurator,
        ReceiveSettings
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;
        Action<EventProcessorClientOptions> _configureOptions;
        Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;
        readonly PipeConfigurator<IEventHubProcessorContext> _processorConfigurator;
        string _containerName;

        public EventHubReceiveEndpointConfigurator(string eventHubName, string consumerGroup, IHostSettings hostSettings, IStorageSettings storageSettings,
            IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            EventHubName = eventHubName;
            ConsumerGroup = consumerGroup;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            ConcurrencyLimit = 1;

            _processorConfigurator = new PipeConfigurator<IEventHubProcessorContext>();
        }

        public string ContainerName
        {
            get => _containerName;
            set => _containerName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TimeSpan CheckpointInterval { get; set; }

        public string ConsumerGroup { get; }

        public string EventHubName { get; }

        public ushort CheckpointMessageCount { get; set; }

        public int ConcurrencyLimit { get; set; }

        public Action<EventProcessorClientOptions> ConfigureOptions
        {
            set => _configureOptions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void OnPartitionClosing(Func<PartitionClosingEventArgs, Task> handler)
        {
            if (_partitionClosingHandler != null)
                throw new InvalidOperationException("Partition closing event handler may not be specified more than once.");
            _partitionClosingHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void OnPartitionInitializing(Func<PartitionInitializingEventArgs, Task> handler)
        {
            if (_partitionInitializingHandler != null)
                throw new InvalidOperationException("Partition initializing event handler may not be specified more than once.");
            _partitionInitializingHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public IReceiveEndpointControl Build()
        {
            IEventHubReceiveEndpointContext CreateContext()
            {
                var builder = new EventHubReceiveEndpointBuilder(_busInstance, _endpointConfiguration, _hostSettings, _storageSettings, this, _configureOptions,
                    _partitionClosingHandler, _partitionInitializingHandler);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return builder.CreateReceiveEndpointContext();
            }

            var context = CreateContext();

            _processorConfigurator.UseFilter(new EvenHubBlobContainerCreatorFilter());
            _processorConfigurator.UseFilter(new EventHubConsumerFilter(context));

            IPipe<IEventHubProcessorContext> processorPipe = _processorConfigurator.Build();

            var transport = new ReceiveTransport<IEventHubProcessorContext>(_busInstance.HostConfiguration, context, () => context.ContextSupervisor,
                processorPipe);

            return new ReceiveEndpoint(transport, context);
        }
    }
}
