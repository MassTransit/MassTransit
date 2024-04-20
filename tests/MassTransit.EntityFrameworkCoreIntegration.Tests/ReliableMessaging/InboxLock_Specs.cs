namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Tests.ReliableMessaging;
    using MassTransit.Tests.ReliableMessaging.InboxLock;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class When_multiple_deliveries_of_the_same_message_occur
    {
        [Test]
        public async Task Should_block_subsequent_consumers_by_lock()
        {
            await using var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            var messageId = NewId.NextGuid();

            await harness.Bus.Publish(new Command(), x => x.MessageId = messageId);
            await Task.Delay(500);

            await harness.Bus.Publish(new Command(), x => x.MessageId = messageId);
            await Task.Delay(50);

            await harness.Bus.Publish(new Command(), x => x.MessageId = messageId);

            await harness.InactivityTask;

            var count = await harness.Consumed.SelectAsync<Event>().Count();

            Assert.That(count, Is.EqualTo(100));

            var sentCount = await harness.Sent.SelectAsync(x => true).Count();
            Assert.That(sentCount, Is.EqualTo(100));

            var events = provider.GetRequiredService<IList<Event>>();

            Assert.That(events, Has.Count.EqualTo(100));
        }
    }


    public static class InboxLockEntityFrameworkTestExtensions
    {
        public static IServiceCollection AddEntityFrameworkInMemoryTestHarness(this IServiceCollection services)
        {
            services
                .AddDbContext<ReliableDbContext>(builder =>
                {
                    ReliableDbContextFactory.Apply(builder);
                })
                .AddSingleton<IList<Event>, List<Event>>()
                .AddHostedService<MigrationHostedService<ReliableDbContext>>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>();

                    x.AddHandler(async (Event message, IList<Event> events) =>
                    {
                        lock (events)
                            events.Add(message);
                    });
                    x.AddConsumer<InboxLockConsumer, InboxLockEntityFrameworkConsumerDefinition>();
                });

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }


        public class InboxLockEntityFrameworkConsumerDefinition :
            ConsumerDefinition<InboxLockConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<InboxLockConsumer> consumerConfigurator, IRegistrationContext context)
            {
                endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

                endpointConfigurator.UseEntityFrameworkOutbox<ReliableDbContext>(context);
            }
        }
    }
}
