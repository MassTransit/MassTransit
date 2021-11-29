namespace MassTransit.Transports
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Courier;


    public class ReceiveEndpointDispatcherFactory :
        IReceiveEndpointDispatcherFactory
    {
        readonly ConcurrentDictionary<string, Lazy<IReceiveEndpointDispatcher>> _dispatchers;
        readonly IHostConfiguration _hostConfiguration;
        readonly IBusRegistrationContext _registration;

        public ReceiveEndpointDispatcherFactory(IBusRegistrationContext registration, IBusInstance busInstance)
        {
            _hostConfiguration = busInstance.HostConfiguration;
            _registration = registration;

            _dispatchers = new ConcurrentDictionary<string, Lazy<IReceiveEndpointDispatcher>>();
        }

        public IReceiveEndpointDispatcher CreateReceiver(string queueName)
        {
            return CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });
        }

        public IReceiveEndpointDispatcher CreateConsumerReceiver<T>(string queueName)
            where T : class, IConsumer
        {
            return CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureConsumer<T>(_registration);
            });
        }

        public IReceiveEndpointDispatcher CreateSagaReceiver<T>(string queueName)
            where T : class, ISaga
        {
            return CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureSaga<T>(_registration);
            });
        }

        public IReceiveEndpointDispatcher CreateExecuteActivityReceiver<T>(string queueName)
            where T : class, IExecuteActivity
        {
            return CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureExecuteActivity(_registration, typeof(T));
            });
        }

        public ValueTask DisposeAsync()
        {
            IEnumerable<IReceiveEndpointDispatcher> dispatchers = _dispatchers.Values.Where(x => x.IsValueCreated).Select(x => x.Value);
            foreach (var dispatcher in dispatchers)
            {
                var metrics = dispatcher.GetMetrics();
                LogContext.Debug?.Log("Dispatcher completed {InputAddress}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent",
                    dispatcher.InputAddress, metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }

            return default;
        }

        IReceiveEndpointDispatcher CreateMessageReceiver(string queueName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentNullException(nameof(queueName));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return _dispatchers.GetOrAdd(queueName, name => new Lazy<IReceiveEndpointDispatcher>(() =>
            {
                var endpointConfiguration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

                var configurator = endpointConfiguration as IReceiveEndpointConfigurator ??
                    throw new ConfigurationException("The receive endpoint configuration was not valid");

                configurator.ThrowOnSkippedMessages();
                configurator.RethrowFaultedMessages();
                configurator.PublishFaults = false;
                configurator.ConfigureConsumeTopology = false;

                var configureReceiveEndpoint = _registration.GetConfigureReceiveEndpoints();

                configureReceiveEndpoint.Configure(queueName, configurator);

                configure(configurator);

                IReadOnlyList<ValidationResult> result = endpointConfiguration.Validate()
                    .ThrowIfContainsFailure("The endpoint configuration is invalid:");

                try
                {
                    var receiveEndpointContext = endpointConfiguration.CreateReceiveEndpointContext();

                    return new ReceiveEndpointDispatcher(receiveEndpointContext);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationException(result, "An exception occurred during dispatcher creation", ex);
                }
            })).Value;
        }
    }
}
