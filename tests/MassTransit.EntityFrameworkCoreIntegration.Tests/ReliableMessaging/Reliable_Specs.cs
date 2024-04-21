#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Tests.ReliableMessaging;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;
    using Transports.Fabric;


    public class Reliable_Specs
    {
        [Test]
        public async Task Should_startup_and_shutdown()
        {
            await using var provider = new ServiceCollection()
                .AddReliableTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(30);

            await harness.Start();

            try
            {
                var messageId = NewId.NextGuid();

                Task<ConsumeContext<Event>> first =
                    await harness.ConnectPublishHandler<Event>(context => context.Message.MessageId == messageId && context.Message.Text == "First");
                Task<ConsumeContext<Event>> second =
                    await harness.ConnectPublishHandler<Event>(context => context.Message.MessageId == messageId && context.Message.Text == "Second");

                await harness.Bus.Publish<Command>(new { messageId }, x => x.MessageId = messageId);

                Assert.That(await harness.GetConsumerHarness<ReliableConsumer>().Consumed.Any<Command>(), Is.True);

                await first;

                await second;
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_handle_failure_in_the_consumer()
        {
            await using var provider = new ServiceCollection()
                .AddReliableTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(30);

            await harness.Start();

            try
            {
                var messageId = NewId.NextGuid();

                Task<ConsumeContext<Event>> first =
                    await harness.ConnectPublishHandler<Event>(context => context.Message.MessageId == messageId && context.Message.Text == "First");
                Task<ConsumeContext<Event>> second =
                    await harness.ConnectPublishHandler<Event>(context => context.Message.MessageId == messageId && context.Message.Text == "Second");

                await harness.Bus.Publish<Command>(new
                {
                    messageId,
                    FailWhenConsuming = true
                }, x => x.MessageId = messageId);

                Assert.That(await harness.GetConsumerHarness<ReliableConsumer>().Consumed.Any<Command>(), Is.True);

                await first;

                await second;
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_handle_the_saga_successfully()
        {
            await using var provider = new ServiceCollection()
                .AddReliableTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(30);

            await harness.Start();

            try
            {
                var messageId = NewId.NextGuid();
                var sagaId = NewId.NextGuid();

                await harness.Bus.Publish<CreateState>(new { CorrelationId = sagaId }, x => x.MessageId = messageId);

                ISagaStateMachineTestHarness<ReliableStateMachine, ReliableState>? sagaHarness =
                    harness.GetSagaStateMachineHarness<ReliableStateMachine, ReliableState>();

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(await sagaHarness.Consumed.Any<CreateState>(), Is.True);

                    Assert.That(await sagaHarness.Exists(sagaId, x => x.Verified), Is.Not.Null);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_handle_the_first_attempt_failure()
        {
            await using var provider = new ServiceCollection()
                .AddReliableTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(30);

            await harness.Start();

            try
            {
                var messageId = NewId.NextGuid();
                var sagaId = NewId.NextGuid();

                await harness.Bus.Publish(new CreateState
                {
                    CorrelationId = sagaId,
                    FailOnFirstAttempt = true
                }, x => x.MessageId = messageId);

                ISagaStateMachineTestHarness<ReliableStateMachine, ReliableState>? sagaHarness =
                    harness.GetSagaStateMachineHarness<ReliableStateMachine, ReliableState>();

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(await sagaHarness.Consumed.Any<CreateState>(), Is.True);

                    Assert.That(await sagaHarness.Exists(sagaId, x => x.Verified), Is.Not.Null);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_handle_the_delivery_failure()
        {
            await using var provider = new ServiceCollection()
                .AddReliableTestHarness()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(30);

            await harness.Start();

            try
            {
                var messageId = NewId.NextGuid();
                var sagaId = NewId.NextGuid();

                await harness.Bus.Publish(new CreateState
                {
                    CorrelationId = sagaId,
                    FailMessageDelivery = true
                }, x => x.MessageId = messageId);

                ISagaStateMachineTestHarness<ReliableStateMachine, ReliableState>? sagaHarness =
                    harness.GetSagaStateMachineHarness<ReliableStateMachine, ReliableState>();

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(await sagaHarness.Consumed.Any<CreateState>(), Is.True);

                    Assert.That(await sagaHarness.Exists(sagaId, x => x.Verified), Is.Not.Null);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_include_the_routing_key()
        {
            await using var provider = new ServiceCollection()
                .AddReliableTestHarnessWithExchangeRouting()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(30);

            await harness.Start();

            try
            {
                var messageId = NewId.NextGuid();

                await harness.Bus.Publish<Command>(new { messageId }, x => x.MessageId = messageId);

                Assert.That(await harness.GetConsumerHarness<ReliableConsumer>().Consumed.Any<Command>(), Is.True);

                IConsumerTestHarness<ReliableEventConsumer> consumerHarness = harness.GetConsumerHarness<ReliableEventConsumer>();

                Assert.That(await consumerHarness.Consumed.Any<Event>(), Is.True);

                IReceivedMessage<Event>? context = await consumerHarness.Consumed.SelectAsync<Event>().First();

                Assert.That(context.Context.RoutingKey(), Is.EqualTo("alpha"));
            }
            finally
            {
                await harness.Stop();
            }
        }
    }


    public static class ReliableTestExtensions
    {
        public static IServiceCollection AddReliableTestHarness(this IServiceCollection services)
        {
            services.AddDbContext<ReliableDbContext>(builder =>
                {
                    ReliableDbContextFactory.Apply(builder);
                })
                .AddHostedService<MigrationHostedService<ReliableDbContext>>()
                .AddScoped<IReliableService, ReliableService>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>();

                    x.AddConsumer<ReliableConsumer, ReliableConsumerDefinition>();
                    x.AddSagaStateMachine<ReliableStateMachine, ReliableState, ReliableStateDefinition>()
                        .EntityFrameworkRepository(r => r.ExistingDbContext<ReliableDbContext>());
                });

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }

        public static IServiceCollection AddReliableTestHarnessWithExchangeRouting(this IServiceCollection services)
        {
            services.AddDbContext<ReliableDbContext>(builder =>
                {
                    ReliableDbContextFactory.Apply(builder);
                })
                .AddHostedService<MigrationHostedService<ReliableDbContext>>()
                .AddScoped<IReliableService, ReliableService>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>();

                    x.AddConsumer<ReliableEventConsumer, ReliableEventConsumerDefinition>();
                    x.AddConsumer<ReliableConsumer, ReliableConsumerDefinition>();
                    x.AddSagaStateMachine<ReliableStateMachine, ReliableState, ReliableStateDefinition>()
                        .EntityFrameworkRepository(r => r.ExistingDbContext<ReliableDbContext>());

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.Publish<Event>(t => t.ExchangeType = ExchangeType.Direct);
                        cfg.ConfigureEndpoints(context);
                    });
                });

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }
    }
}
