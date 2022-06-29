namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Diagnostics;
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

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

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
            });
            services.AddHostedService<MigrationHostedService<ReliableDbContext>>();

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }
    }
}
