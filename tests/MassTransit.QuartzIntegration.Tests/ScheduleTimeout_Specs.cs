namespace MassTransit.QuartzIntegration.Tests
{
    namespace ScheduleTimeout_Specs
    {
        using System;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using Saga;
        using Testing;


        [TestFixture]
        public class Scheduling_a_message_from_a_state_machine :
            QuartzInMemoryTestFixture
        {
            [Test]
            public async Task Should_cancel_when_the_order_is_submitted()
            {
                Task<ConsumeContext<CartRemoved>> handler = await ConnectPublishHandler<CartRemoved>();

                var memberNumber = NewId.NextGuid().ToString();

                await InputQueueSendEndpoint.Send<CartItemAdded>(new {MemberNumber = memberNumber});

                Guid? saga = await _repository.ShouldContainSagaInState(x => x.MemberNumber == memberNumber, _machine, _machine.Active, TestTimeout);

                Assert.IsTrue(saga.HasValue);

                await InputQueueSendEndpoint.Send<OrderSubmitted>(new {MemberNumber = memberNumber});

                ConsumeContext<CartRemoved> removed = await handler;

                await Task.Delay(3000);
            }

            [Test]
            public async Task Should_receive_the_timeout()
            {
                Task<ConsumeContext<CartRemoved>> handler = await ConnectPublishHandler<CartRemoved>();

                var memberNumber = NewId.NextGuid().ToString();

                await InputQueueSendEndpoint.Send<CartItemAdded>(new {MemberNumber = memberNumber});

                ConsumeContext<CartRemoved> removed = await handler;
            }

            [Test]
            public async Task Should_reschedule_the_timeout_when_items_are_added()
            {
                Task<ConsumeContext<CartRemoved>> handler = await ConnectPublishHandler<CartRemoved>();

                var memberNumber = NewId.NextGuid().ToString();

                await InputQueueSendEndpoint.Send<CartItemAdded>(new {MemberNumber = memberNumber});

                Guid? saga = await _repository.ShouldContainSagaInState(x => x.MemberNumber == memberNumber, _machine, _machine.Active, TestTimeout);

                Assert.IsTrue(saga.HasValue);

                await InputQueueSendEndpoint.Send<CartItemAdded>(new {MemberNumber = memberNumber});

                ConsumeContext<CartRemoved> removed = await handler;
            }

            InMemorySagaRepository<TestState> _repository;
            TestStateMachine _machine;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInMemoryReceiveEndpoint(configurator);

                _repository = new InMemorySagaRepository<TestState>();

                _machine = new TestStateMachine();

                configurator.StateMachineSaga(_machine, _repository);
            }
        }


        class TestState :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public string MemberNumber { get; set; }

            public Guid? CartTimeoutTokenId { get; set; }

            public int ExpiresAfterSeconds { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public interface CartItemAdded
        {
            string MemberNumber { get; }
        }


        public interface CartRemoved
        {
            string MemberNumber { get; }
        }


        public interface CartExpired
        {
            string MemberNumber { get; }
        }


        public interface OrderSubmitted
        {
            string MemberNumber { get; }
        }


        class TestStateMachine :
            MassTransitStateMachine<TestState>
        {
            public TestStateMachine()
            {
                Event(() => ItemAdded, x => x.CorrelateBy(p => p.MemberNumber, p => p.Message.MemberNumber)
                    .SelectId(context => NewId.NextGuid()));

                Event(() => Submitted, x => x.CorrelateBy(p => p.MemberNumber, p => p.Message.MemberNumber));

                Schedule(() => CartTimeout, x => x.CartTimeoutTokenId, x =>
                {
                    x.Delay = TimeSpan.FromSeconds(30);
                    x.Received = p => p.CorrelateBy(state => state.MemberNumber, context => context.Message.MemberNumber);
                });


                Initially(When(ItemAdded)
                    .ThenAsync(context =>
                    {
                        context.Instance.MemberNumber = context.Data.MemberNumber;
                        context.Instance.ExpiresAfterSeconds = 3;
                        return Console.Out.WriteLineAsync($"Cart {context.Instance.CorrelationId} Created: {context.Data.MemberNumber}");
                    })
                    .Schedule(CartTimeout, context => context.Init<CartExpired>(context.Instance),
                        context => TimeSpan.FromSeconds(context.Instance.ExpiresAfterSeconds))
                    .TransitionTo(Active));

                During(Active,
                    When(CartTimeout.Received)
                        .ThenAsync(context => Console.Out.WriteLineAsync($"Cart Expired: {context.Data.MemberNumber}"))
                        .PublishAsync(context => context.Init<CartRemoved>(context.Instance))
                        .Finalize(),
                    When(Submitted)
                        .ThenAsync(context => Console.Out.WriteLineAsync($"Cart Submitted: {context.Data.MemberNumber}"))
                        .Unschedule(CartTimeout)
                        .PublishAsync(context => context.Init<CartRemoved>(context.Instance))
                        .Finalize(),
                    When(ItemAdded)
                        .ThenAsync(context => Console.Out.WriteLineAsync($"Card item added: {context.Data.MemberNumber}"))
                        .Schedule(CartTimeout, context => context.Init<CartExpired>(context.Instance),
                            context => TimeSpan.FromSeconds(context.Instance.ExpiresAfterSeconds)));

                SetCompletedWhenFinalized();
            } // ReSharper disable UnassignedGetOnlyAutoProperty
            public Schedule<TestState, CartExpired> CartTimeout { get; }

            public Event<CartItemAdded> ItemAdded { get; }
            public Event<OrderSubmitted> Submitted { get; }

            public State Active { get; }
        }
    }
}
