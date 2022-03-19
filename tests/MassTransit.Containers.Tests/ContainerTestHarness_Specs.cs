namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using HarnessContracts;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;
    using Testing;


    namespace HarnessContracts
    {
        using System;


        public interface SubmitOrder
        {
            Guid OrderId { get; }
            string OrderNumber { get; }
        }


        public interface OrderSubmitted
        {
            Guid OrderId { get; }
            string OrderNumber { get; }
        }
    }


    [TestFixture]
    public class Using_the_container_test_harness
    {
        [Test]
        public async Task Should_have_an_even_cleaner_experience_without_owning_the_container()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<SubmitOrderConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .AddScoped<IConsumerDependency, ConsumerDependency>()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish<SubmitOrder>(new
            {
                InVar.CorrelationId,
                OrderNumber = "123"
            });

            await harness.Consumed.Any<SubmitOrder>();

            await harness.Published.Any<OrderSubmitted>();
        }


        interface IConsumerDependency
        {
            Task NotifyOrderSubmitted(Guid orderId, string orderNumber);
        }


        class ConsumerDependency :
            IConsumerDependency
        {
            readonly IPublishEndpoint _publishEndpoint;

            public ConsumerDependency(IPublishEndpoint publishEndpoint)
            {
                _publishEndpoint = publishEndpoint;
            }

            public Task NotifyOrderSubmitted(Guid orderId, string orderNumber)
            {
                return _publishEndpoint.Publish<OrderSubmitted>(new
                {
                    correlationId = orderId,
                    orderNumber
                });
            }
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            readonly IConsumerDependency _dependency;

            public SubmitOrderConsumer(IConsumerDependency dependency)
            {
                _dependency = dependency;
            }

            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return _dependency.NotifyOrderSubmitted(context.Message.OrderId, context.Message.OrderNumber);
            }
        }
    }


    [TestFixture]
    public class Using_message_retry
    {
        [Test]
        public async Task Should_only_publish_a_single_fault()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<SubmitOrderConsumer>();

                    x.AddConfigureEndpointsCallback((name, cfg) => cfg.UseMessageRetry(r => r.Immediate(3)));
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(1);

            await harness.Start();

            await harness.Bus.Publish<SubmitOrder>(new
            {
                InVar.CorrelationId,
                OrderNumber = "123"
            });

            Assert.That(await harness.Published.SelectAsync<Fault<SubmitOrder>>().Count(), Is.EqualTo(1));

            Assert.That(await harness.Consumed.SelectAsync<SubmitOrder>().Count(), Is.EqualTo(1));

            IConsumerTestHarness<SubmitOrderConsumer> consumerHarness = harness.GetConsumerHarness<SubmitOrderConsumer>();

            Assert.That(await consumerHarness.Consumed.SelectAsync<SubmitOrder>().Count(), Is.EqualTo(1));

            Assert.That(SubmitOrderConsumer.Attempts, Is.EqualTo(4));
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public static int Attempts;

            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                Interlocked.Increment(ref Attempts);

                throw new IntentionalTestException();
            }
        }
    }


    [TestFixture]
    public class Using_the_generic_request_client_from_the_harness
    {
        [Test]
        public async Task Should_properly_resolve_within_the_scope()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<SubmitOrderConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var client = harness.GetRequestClient<SubmitOrder>();

            await client.GetResponse<OrderSubmitted>(new
            {
                OrderId = InVar.Id,
                OrderNumber = "123"
            });

            Assert.IsTrue(await harness.Sent.Any<OrderSubmitted>());

            Assert.IsTrue(await harness.Consumed.Any<SubmitOrder>());
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public Task Consume(ConsumeContext<SubmitOrder> context)
            {
                return context.RespondAsync<OrderSubmitted>(context.Message);
            }
        }
    }


    [TestFixture]
    public class Using_the_container_test_harness_with_a_saga_state_machine
    {
        [Test]
        public async Task Should_have_a_simple_clean_syntax()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>();
                })
                .AddScoped<PublishTestStartedActivity>()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var correlationId = NewId.NextGuid();

            await (await harness.GetSagaEndpoint<TestInstance>()).Send(new StartTest
            {
                CorrelationId = correlationId,
                TestKey = "Unique"
            });

            Assert.IsTrue(await harness.Consumed.Any<StartTest>());
            IReceivedMessage<StartTest> startTest = await harness.Consumed.SelectAsync<StartTest>().First();
            Assert.That(startTest.Context.CorrelationId, Is.EqualTo(correlationId));

            Assert.IsTrue(await harness.Published.Any<TestStarted>());
            IPublishedMessage<TestStarted> testStarted = await harness.Published.SelectAsync<TestStarted>().First();
            Assert.That(testStarted.Context.InitiatorId, Is.EqualTo(correlationId));

            Assert.That(startTest.Context.ConversationId, Is.EqualTo(testStarted.Context.ConversationId));
        }
    }
}
