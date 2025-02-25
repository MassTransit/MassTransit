namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using NUnit.Framework;
    using Outbox;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class Using_the_bus_outbox
    {
        [Test]
        [Explicit]
        public async Task Fill_up_the_outbox()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1";
                        o.DatabaseName = "sagaTest";

                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var totalTimer = Stopwatch.StartNew();
            var sendTimer = Stopwatch.StartNew();

            const int loopCount = 100;
            const int messagesPerLoop = 1;
            await Task.WhenAll(Enumerable.Range(0, loopCount).Select(async n =>
            {
                // ReSharper disable once AccessToDisposedClosure
                await using var scope = provider.CreateAsyncScope();

                using var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                await dbContext.BeginTransaction(harness.CancellationToken);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await Task.WhenAll(Enumerable.Range(0, messagesPerLoop).Select(_ => publishEndpoint.Publish(new PingMessage())));

                await dbContext.CommitTransaction(harness.CancellationToken);
            }));

            sendTimer.Stop();

            var count = await harness.Consumed.SelectAsync<PingMessage>().Count();

            totalTimer.Stop();

            var totalTime = totalTimer.Elapsed - harness.TestInactivityTimeout;

            const int messageCount = loopCount * messagesPerLoop;

            Assert.That(count, Is.EqualTo(messageCount));

            TestContext.Out.WriteLine("Message Count: {0}", messageCount);

            TestContext.Out.WriteLine("Total send duration: {0:g}", sendTimer.Elapsed);
            TestContext.Out.WriteLine("Send message rate: {0:F2} (msg/s)", messageCount * 1000 / sendTimer.Elapsed.TotalMilliseconds);
            TestContext.Out.WriteLine("Total consume duration: {0:g}", totalTime);
            TestContext.Out.WriteLine("Consume message rate: {0:F2} (msg/s)", messageCount * 1000 / totalTime.TotalMilliseconds);


            await harness.Stop();
        }

        [Test]
        public async Task Should_not_send_when_transaction_aborted()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1";
                        o.DatabaseName = "sagaTest";

                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await ClearOutbox(provider, harness);

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                await dbContext.AbortTransaction(harness.CancellationToken);
            }

            using var anotherCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(anotherCts.Token), Is.False);

            await harness.Stop();
        }

        [Test]
        public async Task Should_start_immediately_when_context_disposed()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1";
                        o.DatabaseName = "sagaTest";

                        o.QueryDelay = TimeSpan.FromMinutes(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await ClearOutbox(provider, harness);

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                await using var scope = harness.Scope.ServiceProvider.CreateAsyncScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                await dbContext.BeginTransaction(harness.CancellationToken);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                await dbContext.CommitTransaction(harness.CancellationToken);
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);

            await harness.Stop();
        }

        [Test]
        public async Task Should_support_delayed_message_scheduler()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1";
                        o.DatabaseName = "sagaTest";
                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            await harness.Start();

            await ClearOutbox(provider, harness);

            Guid scheduledId = NewId.NextGuid();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                await dbContext.BeginTransaction(harness.CancellationToken);

                var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

                await scheduler.SchedulePublish(TimeSpan.FromSeconds(8), new PingMessage(scheduledId));

                await dbContext.CommitTransaction(harness.CancellationToken);
            }

            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(x => x.Context.Message.CorrelationId == scheduledId, cts.Token), Is.False);
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(x => x.Context.Message.CorrelationId == scheduledId), Is.True);
        }

        [Test]
        public async Task Should_support_multiple_transactions()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1";
                        o.DatabaseName = "sagaTest";

                        o.QueryDelay = TimeSpan.FromMinutes(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await ClearOutbox(provider, harness);

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                await using var scope = harness.Scope.ServiceProvider.CreateAsyncScope();
                using var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                await dbContext.BeginTransaction(harness.CancellationToken);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var firstId = NewId.NextGuid();
                await publishEndpoint.Publish(new PingMessage(firstId));

                using var cts1 = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(x => x.Context.Message.CorrelationId == firstId, cts1.Token), Is.False);

                await dbContext.CommitTransaction(harness.CancellationToken);

                var secondId = NewId.NextGuid();

                await publishEndpoint.Publish(new PingMessage(secondId));

                using var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(x => x.Context.Message.CorrelationId == firstId, cts2.Token), Is.True);

                await dbContext.CommitTransaction(harness.CancellationToken);
            }

            Assert.That(consumerHarness.Consumed.Count(harness.CancellationToken), Is.EqualTo(2));

            await harness.Stop();
        }

        [Test]
        public async Task Should_support_the_test_harness()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1";
                        o.DatabaseName = "sagaTest";

                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await ClearOutbox(provider, harness);

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                await dbContext.BeginTransaction(harness.CancellationToken);

                var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                await dbContext.CommitTransaction(harness.CancellationToken);
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);

            await harness.Stop();
        }

        [Test]
        public async Task Should_work_without_starting_the_bus()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1";
                        o.DatabaseName = "sagaTest";
                        o.DisableInboxCleanupService();

                        o.UseBusOutbox(bo => bo.DisableDeliveryService());
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .RemoveMassTransitHostedService()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await ClearOutbox(provider, harness);

            {
                using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                await dbContext.BeginTransaction(harness.CancellationToken);

                var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                await dbContext.CommitTransaction(harness.CancellationToken);
            }

            {
                await using var scope = provider.CreateAsyncScope();

                using var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                MongoDbCollectionContext<OutboxMessage> collection = dbContext.GetCollection<OutboxMessage>();

                var count = await collection.Find(Builders<OutboxMessage>.Filter.Empty).CountDocumentsAsync();

                Assert.That(count, Is.EqualTo(1));
            }

            await harness.Stop();
        }

        static async Task ClearOutbox(IServiceProvider provider, ITestHarness harness)
        {
            await using var scope = provider.CreateAsyncScope();

            using var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            MongoDbCollectionContext<OutboxMessage> collection = dbContext.GetCollection<OutboxMessage>();

            await collection.DeleteMany(Builders<OutboxMessage>.Filter.Empty, harness.CancellationToken);
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    public static class BusOutboxTestExtensions
    {
        public static IServiceCollection AddBusOutboxServices(this IServiceCollection services)
        {
            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }
    }
}
