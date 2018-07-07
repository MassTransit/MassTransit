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

        public IMessageReceiver CreateMessageReceiver(string entityPath)
        {
            var messageReceiver = new MessageReceiver(
                ServiceBusConnection.Value,
                entityPath,
                ReceiveMode.PeekLock,
                RetryPolicy
            );

            return messageReceiver;
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

        public Lazy<ServiceBusConnection> ServiceBusConnection => new Lazy<ServiceBusConnection>(
            () =>
            {
                var connection = new ServiceBusConnection(Address.ToString(), Settings.TransportType, RetryPolicy)
                {
                    TokenProvider = Settings.TokenProvider
                };

                return connection;
            }); 

        public Uri Address { get; set; }
        public RetryPolicy RetryPolicy { get; set; }

        public async Task CloseAsync()
        {
            if (ServiceBusConnection.IsValueCreated)
            {
                await ServiceBusConnection.Value.CloseAsync();
            }
        }

        public static Task<MessagingFactory> CreateAsync(Uri serviceUri, MessagingFactorySettings settings)
        {
            var factory = new MessagingFactory(serviceUri, settings);

            return Task.FromResult(factory);
        }
    }
}