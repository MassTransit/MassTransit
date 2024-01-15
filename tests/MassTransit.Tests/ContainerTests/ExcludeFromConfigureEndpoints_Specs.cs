namespace MassTransit.Tests.ContainerTests
{
    using System;
    using System.Threading.Tasks;
    using Common_Tests;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    public class ExcludeConsumerFromConfigureEndpoints_Specs
    {
        [Test]
        public async Task Should_exclude_consumer_explicitly()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<ExcludedConsumer>()
                        .ExcludeFromConfigureEndpoints();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish<Ping>(new { }, harness.CancellationToken);

            Assert.That(await harness.Consumed.Any<Ping>(), Is.False);
        }

        [Test]
        public async Task Should_exclude_consumer_by_attribute()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x => x.AddConsumer<ExcludedByAttributeConsumer>())
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish<Ping>(new { }, harness.CancellationToken);

            Assert.That(await harness.Consumed.Any<Ping>(), Is.False);
        }


        class ExcludedConsumer :
            IConsumer<Ping>
        {
            public Task Consume(ConsumeContext<Ping> context)
            {
                return Task.CompletedTask;
            }
        }


        [ExcludeFromConfigureEndpoints]
        class ExcludedByAttributeConsumer :
            IConsumer<Ping>
        {
            public Task Consume(ConsumeContext<Ping> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    public class ExcludeSagaFromConfigureEndpoints_Specs
    {
        [Test]
        public async Task Should_exclude_saga_explicitly()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSaga<ExcludedSaga>()
                        .ExcludeFromConfigureEndpoints();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new InitiateMessage(), harness.CancellationToken);

            Assert.That(await harness.Consumed.Any<InitiateMessage>(), Is.False);
        }

        [Test]
        public async Task Should_exclude_saga_by_attribute()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSaga<ExcludedByAttributeSaga>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new InitiateMessage(), harness.CancellationToken);

            Assert.That(await harness.Consumed.Any<Ping>(), Is.False);
        }

        [Test]
        public async Task Should_exclude_state_machine_explicitly()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSagaStateMachine<ExcludedStateMachine, ExcludedSagaInstance>()
                        .ExcludeFromConfigureEndpoints();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new InitiateMessage(), harness.CancellationToken);

            Assert.That(await harness.Consumed.Any<InitiateMessage>(), Is.False);
        }

        [Test]
        public async Task Should_exclude_state_machine_by_instance_attribute()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSagaStateMachine<ExcludedStateMachineWithInstanceAttribute, ExcludedStaInstanceByAttribute>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new InitiateMessage(), harness.CancellationToken);

            Assert.That(await harness.Consumed.Any<Ping>(), Is.False);
        }


        class InitiateMessage :
            CorrelatedBy<Guid>
        {
            public InitiateMessage()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }


        class ExcludedSagaInstance : SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        [ExcludeFromConfigureEndpoints]
        class ExcludedStaInstanceByAttribute : ExcludedSagaInstance
        {
        }


        class ExcludedSaga :
            ISaga,
            InitiatedBy<InitiateMessage>
        {
            public Task Consume(ConsumeContext<InitiateMessage> context)
            {
                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }
        }


        [ExcludeFromConfigureEndpoints]
        class ExcludedByAttributeSaga :
            ISaga,
            InitiatedBy<InitiateMessage>
        {
            public Task Consume(ConsumeContext<InitiateMessage> context)
            {
                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }
        }


        class ExcludedStateMachine :
            MassTransitStateMachine<ExcludedSagaInstance>
        {
            public ExcludedStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(When(Initiated).Finalize());

                SetCompletedWhenFinalized();
            }

            public Event<InitiateMessage> Initiated { get; }
        }


        class ExcludedStateMachineWithInstanceAttribute :
            MassTransitStateMachine<ExcludedStaInstanceByAttribute>
        {
            public ExcludedStateMachineWithInstanceAttribute()
            {
                InstanceState(x => x.CurrentState);

                Initially(When(Initiated).Finalize());

                SetCompletedWhenFinalized();
            }

            public Event<InitiateMessage> Initiated { get; }
        }
    }
}
