namespace MassTransit.ServiceBusIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;
    using Transports;


    public class MessageReceiver :
        IMessageReceiver
    {
        const string PathDelimiter = @"/";
        const string SubscriptionsSubPath = "Subscriptions";

        readonly IAsyncBusHandle _busHandle;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ConcurrentDictionary<string, Lazy<IServiceBusMessageReceiver>> _receivers;
        readonly IBusRegistrationContext _registration;

        public MessageReceiver(IBusRegistrationContext registration, IAsyncBusHandle busHandle, IBusInstance busInstance)
        {
            _hostConfiguration = busInstance.HostConfiguration as IServiceBusHostConfiguration
                ?? throw new ConfigurationException("The hostConfiguration was not properly configured for Azure Service Bus");

            _registration = registration;
            _busHandle = busHandle;

            _receivers = new ConcurrentDictionary<string, Lazy<IServiceBusMessageReceiver>>();
        }

        public Task Handle(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            var receiver = CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task Handle(string topicPath, string subscriptionName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            var receiver = CreateMessageReceiver(topicPath, subscriptionName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleConsumer<TConsumer>(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer
        {
            var receiver = CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureConsumer<TConsumer>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleConsumer<TConsumer>(string topicPath, string subscriptionName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer
        {
            var receiver = CreateMessageReceiver(topicPath, subscriptionName, cfg =>
            {
                cfg.ConfigureConsumer<TConsumer>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleSaga<TSaga>(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TSaga : class, ISaga
        {
            var receiver = CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureSaga<TSaga>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleSaga<TSaga>(string topicPath, string subscriptionName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TSaga : class, ISaga
        {
            var receiver = CreateMessageReceiver(topicPath, subscriptionName, cfg =>
            {
                cfg.ConfigureSaga<TSaga>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleExecuteActivity<TActivity>(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TActivity : class
        {
            var receiver = CreateMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureExecuteActivity(_registration, typeof(TActivity));
            });

            return receiver.Handle(message, cancellationToken);
        }

        public void Dispose()
        {
        }

        IServiceBusMessageReceiver CreateMessageReceiver(string queueName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentNullException(nameof(queueName));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return _receivers.GetOrAdd(queueName, name => new Lazy<IServiceBusMessageReceiver>(() =>
            {
                var endpointConfiguration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

                var configurator = new QueueBrokeredMessageReceiverConfiguration(_hostConfiguration, endpointConfiguration);

                configure(configurator);

                return configurator.Build();
            })).Value;
        }

        IServiceBusMessageReceiver CreateMessageReceiver(string topicPath, string subscriptionName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(topicPath))
                throw new ArgumentNullException(nameof(topicPath));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var subscriptionPath = string.Concat(topicPath, PathDelimiter, SubscriptionsSubPath, PathDelimiter, subscriptionName);

            return _receivers.GetOrAdd(subscriptionPath, name => new Lazy<IServiceBusMessageReceiver>(() =>
            {
                var topicConfigurator = new ServiceBusTopicConfigurator(topicPath, false);

                static void NoConfigure(IServiceBusSubscriptionEndpointConfigurator _)
                {
                }

                var endpointConfiguration = _hostConfiguration.CreateSubscriptionEndpointConfiguration(subscriptionName, topicConfigurator.Path, NoConfigure);

                var configurator = new SubscriptionBrokeredMessageReceiverConfiguration(_hostConfiguration, endpointConfiguration);

                configure(configurator);

                return configurator.Build();
            })).Value;
        }
    }
}
