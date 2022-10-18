namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;


    public class Using_the_bus_outbox
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
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
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            try
            {
                {
                    await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var activity = TraceConfig.Source.StartActivity(ActivityKind.Client);

                    await publishEndpoint.Publish(new PingMessage());

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                    Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    activity.Stop();
                }

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        [Explicit]
        public async Task Fill_up_the_outbox()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);
                        o.DisableInboxCleanupService();

                        //                        o.UsePostgres();

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
            const int messagesPerLoop = 3;
            await Task.WhenAll(Enumerable.Range(0, loopCount).Select(async n =>
            {
                // ReSharper disable once AccessToDisposedClosure
                using var scope = provider.CreateScope();

                await using var dbContext = scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await Task.WhenAll(Enumerable.Range(0, messagesPerLoop).Select(_ => publishEndpoint.Publish(new PingMessage())));

                await dbContext.SaveChangesAsync(harness.CancellationToken);
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
        [Explicit]
        public async Task Should_support_delayed_message_scheduler()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
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

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

                await scheduler.SchedulePublish(TimeSpan.FromSeconds(8), new PingMessage());

                await dbContext.SaveChangesAsync(harness.CancellationToken);
            }

            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);
        }

        [Explicit]
        [Test]
        public async Task Should_work_without_starting_the_bus()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.DisableInboxCleanupService();

                        o.UseBusOutbox(bo => bo.DisableDeliveryService());
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .RemoveMassTransitHostedService()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            {
                await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                await dbContext.SaveChangesAsync(harness.CancellationToken);
            }

            {
                using var scope = provider.CreateScope();

                await using var dbContext = scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var count = await dbContext.Set<OutboxMessage>().CountAsync();

                Assert.That(count, Is.EqualTo(1));
            }

            await harness.Stop();
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
            services.AddDbContext<ReliableDbContext>(builder =>
            {
                ReliableDbContextFactory.Apply(builder);
                //
                // builder.UseNpgsql("host=localhost;user id=postgres;password=Password12!;database=MassTransitUnitTests;", options =>
                // {
                //     options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                //     options.MigrationsHistoryTable($"__{nameof(ReliableDbContext)}");
                //
                //     options.EnableRetryOnFailure(5);
                //     options.MinBatchSize(1);
                // });
            });
            services.AddHostedService<MigrationHostedService<ReliableDbContext>>();

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }
    }
}
