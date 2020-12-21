namespace MassTransit.EventHubIntegration.Specifications
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Producer;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Registration;
    using Pipeline;
    using Pipeline.Observables;


    public class EventHubProducerSpecification :
        IEventHubProducerConfigurator,
        IEventHubProducerSpecification
    {
        readonly IHostSettings _hostSettings;
        readonly SendObservable _sendObservers;
        readonly ISerializationConfiguration _serializationConfiguration;
        Action<EventHubProducerClientOptions> _configureOptions;
        Action<ISendPipeConfigurator> _configureSend;

        public EventHubProducerSpecification(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
            _serializationConfiguration = new SerializationConfiguration();
            _sendObservers = new SendObservable();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configureSend = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public Action<EventHubProducerClientOptions> ConfigureOptions
        {
            set => _configureOptions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializationConfiguration.SetSerializer(serializerFactory);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                && (string.IsNullOrWhiteSpace(_hostSettings.FullyQualifiedNamespace) || _hostSettings.TokenCredential == null))
                yield return this.Failure("HostSettings", "is invalid");
        }

        public IEventHubProducerSharedContext CreateContext(IBusInstance busInstance)
        {
            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.HostTopology.SendTopology);
            _configureSend?.Invoke(sendConfiguration.Configurator);
            var sendPipe = sendConfiguration.CreatePipe();

            var options = new EventHubProducerClientOptions();
            _configureOptions?.Invoke(options);

            return new EventHubProducerSharedContext(busInstance, _sendObservers, sendPipe, _serializationConfiguration, _hostSettings, options);
        }


        class EventHubProducerSharedContext :
            Agent,
            IEventHubProducerSharedContext
        {
            readonly IBusInstance _busInstance;
            readonly ConcurrentDictionary<string, EventHubProducerClient> _clients;
            readonly IHostSettings _hostSettings;
            readonly EventHubProducerClientOptions _options;
            readonly ISerializationConfiguration _serializationConfiguration;
            readonly Supervisor _transportSupervisor;

            public EventHubProducerSharedContext(IBusInstance busInstance, SendObservable sendObservers, ISendPipe sendPipe,
                ISerializationConfiguration serializationConfiguration, IHostSettings hostSettings, EventHubProducerClientOptions options)
            {
                _transportSupervisor = new Supervisor();
                SendObservers = sendObservers;
                SendPipe = sendPipe;
                _busInstance = busInstance;
                _serializationConfiguration = serializationConfiguration;
                _hostSettings = hostSettings;
                _options = options;
                _clients = new ConcurrentDictionary<string, EventHubProducerClient>();

                busInstance.HostConfiguration.Agent.Completed.ContinueWith(_ => this.Stop(), TaskContinuationOptions.ExecuteSynchronously);
            }

            protected override async Task StopAgent(StopContext context)
            {
                await _transportSupervisor.Stop(context).ConfigureAwait(false);
                await base.StopAgent(context).ConfigureAwait(false);
            }

            public Uri HostAddress => _busInstance.HostConfiguration.HostAddress;
            public ILogContext LogContext => _busInstance.HostConfiguration.SendLogContext;
            public SendObservable SendObservers { get; }
            public ISendPipe SendPipe { get; }
            public IMessageSerializer Serializer => _serializationConfiguration.Serializer;

            public EventHubProducerClient CreateEventHubClient(string eventHubName)
            {
                return _clients.GetOrAdd(eventHubName, CreateClient);
            }

            EventHubProducerClient CreateClient(string eventHubName)
            {
                var client = !string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                    ? new EventHubProducerClient(_hostSettings.ConnectionString, eventHubName, _options)
                    : new EventHubProducerClient(_hostSettings.FullyQualifiedNamespace, eventHubName, _hostSettings.TokenCredential, _options);

                _transportSupervisor.Add(new EventHubProducerClientAgent(client));
                return client;
            }


            class EventHubProducerClientAgent:
                Agent
            {
                readonly EventHubProducerClient _client;

                public EventHubProducerClientAgent(EventHubProducerClient client)
                {
                    _client = client;
                }

                protected override async Task StopAgent(StopContext context)
                {
                    await _client.DisposeAsync().ConfigureAwait(false);
                    await base.StopAgent(context).ConfigureAwait(false);
                }
            }
        }
    }
}
