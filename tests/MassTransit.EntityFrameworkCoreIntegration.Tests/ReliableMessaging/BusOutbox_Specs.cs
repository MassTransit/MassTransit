namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;


    public class Using_the_bus_outbox
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOnRamp<ReliableDbContext>();

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                await dbContext.SaveChangesAsync(harness.CancellationToken);
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);
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
                })
                .AddHostedService<MigrationHostedService<ReliableDbContext>>()
                .AddEntityFrameworkOnRampDeliveryService<ReliableDbContext>(x => x.SweepInterval = TimeSpan.FromSeconds(1))
                .AddSingleton<ILockStatementProvider, SqlServerLockStatementProvider>();

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }
    }
}
