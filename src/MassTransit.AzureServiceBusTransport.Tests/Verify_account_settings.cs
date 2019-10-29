namespace MassTransit.AzureServiceBusTransport.Tests
{
    namespace Verify_account_settings
    {
        using System;
        using System.Diagnostics;
        using System.IO;
        using System.Threading;
        using System.Threading.Tasks;
        using Microsoft.ServiceBus;
        using Microsoft.ServiceBus.Messaging;
        using Microsoft.ServiceBus.Messaging.Amqp;
        using NUnit.Framework;
        using Util;


        [TestFixture]
        public class The_account_credentials_for_unit_tests
        {
            [Test]
            public async Task Should_be_configured_and_working()
            {
                var settings = new TestAzureServiceBusAccountSettings();
                var provider = new SharedAccessKeyTokenProvider(settings);

                TokenProvider tokenProvider = provider.GetTokenProvider();

                Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", Configuration.ServiceNamespace,
                    "MassTransit.AzureServiceBusTransport.Tests");
                var namespaceManager = new NamespaceManager(serviceUri, tokenProvider);
                CreateQueue(namespaceManager, serviceUri, "TestClient");


                CreateHostQueue(tokenProvider);

                var mfs = new MessagingFactorySettings
                {
                    TokenProvider = tokenProvider,
                    OperationTimeout = TimeSpan.FromSeconds(30),
                    TransportType = TransportType.Amqp,
                    AmqpTransportSettings = new AmqpTransportSettings
                    {
                        BatchFlushInterval = TimeSpan.FromMilliseconds(50),
                    },
                };

                MessagingFactory factory =
                    MessagingFactory.Create(
                        ServiceBusEnvironment.CreateServiceUri("sb", Configuration.ServiceNamespace, Environment.MachineName), mfs);

                MessageReceiver receiver = await factory.CreateMessageReceiverAsync("Control");
                receiver.PrefetchCount = 100;

                var done = TaskUtil.GetTask<bool>();
                int count = 0;
                const int limit = 1000;
                receiver.OnMessageAsync(async message =>
                {
                    await message.CompleteAsync();

                    int received = Interlocked.Increment(ref count);
                    if (received == limit)
                        done.TrySetResult(true);
                }, new OnMessageOptions
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 100,
                    AutoRenewTimeout = TimeSpan.FromSeconds(60),
                });

                MessageSender client = await factory.CreateMessageSenderAsync("Control");

                Stopwatch stopwatch = Stopwatch.StartNew();
                Task[] tasks = new Task[limit];
                for (int i = 0; i < limit; i++)
                {
                    tasks[i] = SendAMessage(client);
                }

                await done.Task;
                stopwatch.Stop();

                await Task.WhenAll(tasks);

                await receiver.CloseAsync();

                Console.WriteLine("Performance: {0:F2}/s", limit * 1000 / stopwatch.ElapsedMilliseconds);
            }

            static async Task SendAMessage(MessageSender client)
            {
                using (var ms = new MemoryStream())
                using (var writer = new StreamWriter(ms))
                {
                    writer.Write("Hello World");
                    writer.Flush();
                    ms.Position = 0;

                    var brokeredMessage = new BrokeredMessage(ms)
                    {
                        ContentType = "text/plain",
                    };
                    await client.SendAsync(brokeredMessage);
                }
            }

            void CreateHostQueue(TokenProvider tokenProvider)
            {
                Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", Configuration.ServiceNamespace,
                    Environment.MachineName);
                var namespaceManager = new NamespaceManager(serviceUri, tokenProvider);
                CreateQueue(namespaceManager, serviceUri, "Control");
            }

            void CreateQueue(NamespaceManager namespaceManager, Uri serviceUri, string queueName)
            {
                if (namespaceManager == null)
                    throw new TransportException(serviceUri, "The namespace manager is not available");

                var description = new QueueDescription(queueName)
                {
                    DefaultMessageTimeToLive = TimeSpan.FromDays(365),
                    EnableBatchedOperations = true,
                    LockDuration = TimeSpan.FromMinutes(5),
                    MaxDeliveryCount = 5,
                    EnableDeadLetteringOnMessageExpiration = true,
                    DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),
                };

                try
                {
                    var queueDescription = namespaceManager.CreateQueue(description);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
            }
        }
    }
}
