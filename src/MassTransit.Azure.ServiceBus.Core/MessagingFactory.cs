namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;


    public class MessagingFactory
    {
        public MessagingFactorySettings Settings { get; }

        public MessagingFactory(Uri serviceUri, MessagingFactorySettings settings)
        {
            Address = serviceUri ?? throw new ArgumentNullException(nameof(serviceUri));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IQueueClient CreateQueueClient(string entityPath)
        {
            var queueClient = new QueueClient(ServiceBusConnection.Value, entityPath, ReceiveMode.PeekLock, RetryPolicy)
            {
                OperationTimeout = Settings.OperationTimeout
            };

            return queueClient;
        }

        public ISubscriptionClient CreateSubscriptionClient(string topicPath, string subscriptionName)
        {
            var subscriptionClient = new SubscriptionClient(ServiceBusConnection.Value, topicPath, subscriptionName, ReceiveMode.PeekLock, RetryPolicy)
            {
                OperationTimeout = Settings.OperationTimeout,
            };

            return subscriptionClient;
        }

        public IMessageSender CreateMessageSender(string entityPath)
        {
            var messageSender = new MessageSender(
                ServiceBusConnection.Value,
                entityPath,
                RetryPolicy
            );

            return messageSender;
        }

        public Lazy<ServiceBusConnection> ServiceBusConnection =>
            new Lazy<ServiceBusConnection>(
                () =>
                {
                    var connection = new ServiceBusConnection(Address.ToString(), Settings.TransportType, RetryPolicy)
                    {
                        TokenProvider = Settings.TokenProvider,
                        OperationTimeout = Settings.OperationTimeout,
                    };

                    return connection;
                });

        public Uri Address { get; set; }
        public RetryPolicy RetryPolicy { get; set; }

        public async Task CloseAsync()
        {
            if (ServiceBusConnection.IsValueCreated)
            {
                await ServiceBusConnection.Value.CloseAsync().ConfigureAwait(false);
            }
        }

        public static Task<MessagingFactory> CreateAsync(Uri serviceUri, MessagingFactorySettings settings)
        {
            var factory = new MessagingFactory(serviceUri, settings);

            return Task.FromResult(factory);
        }
    }
}