namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Configuration;
    using Azure.ServiceBus.Core.Topology.Configurators;
    using Azure.ServiceBus.Core.Transport;
    using Microsoft.Azure.ServiceBus;
    using Registration;
    using Saga;


    public class MessageReceiver :
        IMessageReceiver
    {
        readonly IAsyncBusHandle _busHandle;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ConcurrentDictionary<string, Lazy<IBrokeredMessageReceiver>> _receivers;
        readonly IBusRegistrationContext _registration;

        public MessageReceiver(IBusRegistrationContext registration, IAsyncBusHandle busHandle, IBusInstance busInstance)
        {
            _hostConfiguration = busInstance.HostConfiguration as IServiceBusHostConfiguration
                ?? throw new ConfigurationException("The hostConfiguration was not properly configured for Azure Service Bus");

            _registration = registration;
            _busHandle = busHandle;

            _receivers = new ConcurrentDictionary<string, Lazy<IBrokeredMessageReceiver>>();
        }

        public Task Handle(string queueName, Message message, CancellationToken cancellationToken)
        {
            var receiver = CreateBrokeredMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task Handle(string topicPath, string subscriptionName, Message message, CancellationToken cancellationToken)
        {
            var receiver = CreateBrokeredMessageReceiver(topicPath, subscriptionName, cfg =>
            {
                cfg.ConfigureConsumers(_registration);
                cfg.ConfigureSagas(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleConsumer<TConsumer>(string queueName, Message message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer
        {
            var receiver = CreateBrokeredMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureConsumer<TConsumer>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleConsumer<TConsumer>(string topicPath, string subscriptionName, Message message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer
        {
            var receiver = CreateBrokeredMessageReceiver(topicPath, subscriptionName, cfg =>
            {
                cfg.ConfigureConsumer<TConsumer>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleSaga<TSaga>(string queueName, Message message, CancellationToken cancellationToken)
            where TSaga : class, ISaga
        {
            var receiver = CreateBrokeredMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureSaga<TSaga>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleSaga<TSaga>(string topicPath, string subscriptionName, Message message, CancellationToken cancellationToken)
            where TSaga : class, ISaga
        {
            var receiver = CreateBrokeredMessageReceiver(topicPath, subscriptionName, cfg =>
            {
                cfg.ConfigureSaga<TSaga>(_registration);
            });

            return receiver.Handle(message, cancellationToken);
        }

        public Task HandleExecuteActivity<TActivity>(string queueName, Message message, CancellationToken cancellationToken)
            where TActivity : class
        {
            var receiver = CreateBrokeredMessageReceiver(queueName, cfg =>
            {
                cfg.ConfigureExecuteActivity(_registration, typeof(TActivity));
            });

            return receiver.Handle(message, cancellationToken);
        }

        public void Dispose()
        {
        }

        IBrokeredMessageReceiver CreateBrokeredMessageReceiver(string queueName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentNullException(nameof(queueName));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            return _receivers.GetOrAdd(queueName, name => new Lazy<IBrokeredMessageReceiver>(() =>
            {
                var endpointConfiguration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

                var configurator = new QueueBrokeredMessageReceiverConfiguration(_hostConfiguration, endpointConfiguration);

                configure(configurator);

                return configurator.Build();
            })).Value;
        }

        IBrokeredMessageReceiver CreateBrokeredMessageReceiver(string topicPath, string subscriptionName, Action<IReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(topicPath))
                throw new ArgumentNullException(nameof(topicPath));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var subscriptionPath = EntityNameHelper.FormatSubscriptionPath(topicPath, subscriptionName);

            return _receivers.GetOrAdd(subscriptionPath, name => new Lazy<IBrokeredMessageReceiver>(() =>
            {
                var topicConfigurator = new TopicConfigurator(topicPath, false);

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
