namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using InboxLock;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    [Category("Flaky")]
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


    namespace InboxLock
    {
        using System.Linq;


        public class Command
        {
        }


        public class Event
        {
        }


        public class InboxLockConsumer :
            IConsumer<Command>
        {
            public async Task Consume(ConsumeContext<Command> context)
            {
                await Task.WhenAll(Enumerable.Range(0, 100).Select(index =>
                    context.Publish<Event>(new
                    {
                        context.MessageId,
                        Text = $"{index:0000}"
                    })));
            }
        }


        public class InboxLockEntityFrameworkConsumerDefinition :
            ConsumerDefinition<InboxLockConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<InboxLockConsumer> consumerConfigurator, IRegistrationContext context)
            {
                endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

                endpointConfigurator.UseMongoDbOutbox(context);
            }
        }
    }


    public static class InboxLockMongoDbTestExtensions
    {
        public static IServiceCollection AddEntityFrameworkInMemoryTestHarness(this IServiceCollection services)
        {
            services
                .AddSingleton<IList<Event>, List<Event>>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(r =>
                    {
                        r.Connection = "mongodb://127.0.0.1:27021";
                        r.DatabaseName = "sagaTest";
                    });

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
    }
}
