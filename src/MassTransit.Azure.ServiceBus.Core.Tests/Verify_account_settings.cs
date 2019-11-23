// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace Verify_account_settings
    {
        using System;
        using System.Diagnostics;
        using System.Text;
        using System.Threading;
        using System.Threading.Tasks;
        using Contexts;
        using Hosting;
        using Microsoft.Azure.ServiceBus;
        using Microsoft.Azure.ServiceBus.Core;
        using Microsoft.Azure.ServiceBus.Management;
        using Microsoft.Azure.ServiceBus.Primitives;
        using NUnit.Framework;
        using Pipeline;
        using Util;


        [TestFixture]
        public class The_account_credentials_for_unit_tests
        {
            [Test]
            public async Task Should_be_configured_and_working()
            {
                var settings = new TestAzureServiceBusAccountSettings();
                var provider = new SharedAccessKeyTokenProvider(settings);

                var tokenProvider = provider.GetTokenProvider();

                var namespaceSettings = new NamespaceManagerSettings
                {
                    TokenProvider = tokenProvider
                };

                var serviceUri = AzureServiceBusEndpointUriCreator.Create(
                    Configuration.ServiceNamespace,
                    "MassTransit.Azure.ServiceBus.Core.Tests"
                );

                var namespaceManager = new NamespaceManager(serviceUri, namespaceSettings);
                await CreateQueue(namespaceManager, serviceUri, "TestClient");

                await CreateHostQueue(tokenProvider);

                var mfs = new MessagingFactorySettings
                {
                    TokenProvider = tokenProvider,
                    OperationTimeout = TimeSpan.FromSeconds(30),
                    TransportType = TransportType.Amqp
                };

                var factory = await MessagingFactory.CreateAsync(serviceUri, mfs);

                var receiver = factory.CreateQueueClient("Control");
                receiver.PrefetchCount = 100;

                var done = TaskUtil.GetTask<bool>();
                var count = 0;
                const int limit = 1000;
                receiver.RegisterMessageHandler(async (message, cancellationToken) =>
                {
                    await receiver.CompleteAsync(message.SystemProperties.LockToken);

                    var received = Interlocked.Increment(ref count);
                    if (received == limit)
                        done.TrySetResult(true);
                }, new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = 100,
                    MaxAutoRenewDuration = TimeSpan.FromSeconds(60)
                });

                var client = factory.CreateMessageSender("Control");

                var stopwatch = Stopwatch.StartNew();
                var tasks = new Task[limit];
                for (var i = 0; i < limit; i++)
                    tasks[i] = SendAMessage(client);

                await done.Task;
                stopwatch.Stop();

                await Task.WhenAll(tasks);

                await receiver.CloseAsync();

                Console.WriteLine("Performance: {0:F2}/s", limit * 1000 / stopwatch.ElapsedMilliseconds);
            }

            Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
            {
                return Task.CompletedTask;
            }

            static async Task SendAMessage(IMessageSender client)
            {
                var brokeredMessage = new Message(Encoding.UTF8.GetBytes("Hello World"))
                {
                    ContentType = "text/plain"
                };

                await client.SendAsync(brokeredMessage);
            }

            Task CreateHostQueue(ITokenProvider tokenProvider)
            {
                var serviceUri = AzureServiceBusEndpointUriCreator.Create(
                    Configuration.ServiceNamespace,
                    Environment.MachineName
                );

                var settings = new NamespaceManagerSettings
                {
                    TokenProvider = tokenProvider
                };

                var namespaceManager = new NamespaceManager(serviceUri, settings);

                return CreateQueue(namespaceManager, serviceUri, "Control");
            }

            async Task CreateQueue(NamespaceManager namespaceManager, Uri serviceUri, string queueName)
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
                    DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10)
                };

                try
                {
                    var queueDescription = await namespaceManager.CreateQueueAsync(description);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
            }
        }
    }
}
