namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using NUnit.Framework;
    using Outbox;
    using TestFramework.Messages;
    using Testing;


    [Explicit]
    [TestFixture]
    public class Using_the_bus_outbox
    {
        [Test]
        public async Task Should_not_send_when_transaction_aborted()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1:27021";
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

                //                await dbContext.BeginTransaction(harness.CancellationToken);

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
        public async Task Should_support_delayed_message_scheduler()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1:27021";
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

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                await dbContext.BeginTransaction(harness.CancellationToken);

                var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

                await scheduler.SchedulePublish(TimeSpan.FromSeconds(8), new PingMessage());

                await dbContext.CommitTransaction(harness.CancellationToken);
            }

            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);
        }

        [Test]
        public async Task Should_support_the_test_harness()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(o =>
                    {
                        o.Connection = "mongodb://127.0.0.1:27021";
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
                        o.Connection = "mongodb://127.0.0.1:27021";
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
                using var scope = provider.CreateScope();

                using var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

                MongoDbCollectionContext<OutboxMessage> collection = dbContext.GetCollection<OutboxMessage>();

                var count = await collection.Find(Builders<OutboxMessage>.Filter.Empty).CountDocumentsAsync();

                Assert.That(count, Is.EqualTo(1));
            }

            await harness.Stop();
        }

        static async Task ClearOutbox(ServiceProvider provider, ITestHarness harness)
        {
            using var scope = provider.CreateScope();

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
