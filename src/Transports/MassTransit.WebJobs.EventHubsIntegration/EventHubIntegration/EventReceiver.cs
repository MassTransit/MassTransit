namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using AzureServiceBusTransport.Configuration;
    using Configuration;
    using Transports;


    public class EventReceiver :
        IEventReceiver
    {
        readonly IAsyncBusHandle _busHandle;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ConcurrentDictionary<string, IEventDataReceiver> _receivers;
        readonly IBusRegistrationContext _registration;

        public EventReceiver(IBusRegistrationContext registration, IAsyncBusHandle busHandle, IBusInstance busInstance)
        {
            _hostConfiguration = busInstance.HostConfiguration as IServiceBusHostConfiguration
                ?? throw new ConfigurationException("The hostConfiguration was not properly configured for Azure Service Bus");

            _registration = registration;
            _busHandle = busHandle;

            _receivers = new ConcurrentDictionary<string, IEventDataReceiver>();
        }

        public void Dispose()
        {
        }

        public Task Handle(string entityName, EventData message, CancellationToken cancellationToken)
        {
            var receiver = CreateEventDataReceiver(entityName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleConsumer<TConsumer>(string entityName, EventData message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer
        {
            var receiver = CreateEventDataReceiver(entityName, cfg =>
            {
                cfg.ConfigureConsumer<TConsumer>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleSaga<TSaga>(string entityName, EventData message, CancellationToken cancellationToken)
            where TSaga : class, ISaga
        {
            var receiver = CreateEventDataReceiver(entityName, cfg =>
            {
                cfg.ConfigureSaga<TSaga>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleExecuteActivity<TActivity>(string entityName, EventData message, CancellationToken cancellationToken)
            where TActivity : class
        {
            var receiver = CreateEventDataReceiver(entityName, cfg =>
            {
                cfg.ConfigureExecuteActivity(_registration, typeof(TActivity));
            });

            return receiver.Handle(message, cancellationToken);
        }

        IEventDataReceiver CreateEventDataReceiver(string entityName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentNullException(nameof(entityName));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return _receivers.GetOrAdd(entityName, name =>
            {
                var endpointConfiguration = _hostConfiguration.CreateReceiveEndpointConfiguration(entityName);

                var configurator = new EventDataReceiverConfiguration(_hostConfiguration, endpointConfiguration);

                configure(configurator);

                return configurator.Build();
            });
        }
    }
}
