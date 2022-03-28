namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace ScheduleTimeout_Specs
    {
        using System;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using Testing;


        [TestFixture]
        public class Scheduling_a_message_from_a_state_machine :
            AzureServiceBusTestFixture
        {
            [Test]
            [Explicit]
            public async Task Should_cancel_when_the_order_is_submitted()
            {
                var memberNumber = NewId.NextGuid().ToString();

                await InputQueueSendEndpoint.Send<CartItemAdded>(new { MemberNumber = memberNumber });

                Guid? saga = await _repository.ShouldContainSagaInState(x => x.MemberNumber == memberNumber, _machine, _machine.Active, TestTimeout);

                Assert.IsTrue(saga.HasValue);

                await InputQueueSendEndpoint.Send<OrderSubmitted>(new { MemberNumber = memberNumber });

                ConsumeContext<CartRemoved> removed = await _cartRemoved;

                await Task.Delay(3000);
            }

            [Test]
            [Explicit]
            public async Task Should_receive_the_timeout()
            {
                var memberNumber = NewId.NextGuid().ToString();

                await InputQueueSendEndpoint.Send<CartItemAdded>(new { MemberNumber = memberNumber });

                ConsumeContext<CartRemoved> removed = await _cartRemoved;
            }

            [Test]
            [Explicit]
            public async Task Should_reschedule_the_timeout_when_items_are_added()
            {
                var memberNumber = NewId.NextGuid().ToString();

                await InputQueueSendEndpoint.Send<CartItemAdded>(new { MemberNumber = memberNumber });

                Guid? saga = await _repository.ShouldContainSagaInState(x => x.MemberNumber == memberNumber, _machine, _machine.Active, TestTimeout);

                Assert.IsTrue(saga.HasValue);

                await InputQueueSendEndpoint.Send<CartItemAdded>(new { MemberNumber = memberNumber });

                ConsumeContext<CartRemoved> removed = await _cartRemoved;
            }

            InMemorySagaRepository<TestState> _repository;
            TestStateMachine _machine;
            Task<ConsumeContext<CartRemoved>> _cartRemoved;

            protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
            {
                configurator.SubscriptionEndpoint<CartRemoved>("second_queue", x =>
                {
                    _cartRemoved = Handled<CartRemoved>(x);
                });
            }

            protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
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


        class CartExpiredEvent :
            CartExpired
        {
            readonly TestState _state;

            public CartExpiredEvent(TestState state)
            {
                _state = state;
            }

            public string MemberNumber => _state.MemberNumber;
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
                    x.Delay = TimeSpan.FromSeconds(10);
                    x.Received = p => p.CorrelateBy(state => state.MemberNumber, context => context.Message.MemberNumber);
                });


                Initially(When(ItemAdded)
                    .Then(context =>
                    {
                        context.Instance.MemberNumber = context.Data.MemberNumber;
                        context.Instance.ExpiresAfterSeconds = 3;

                        LogContext.Debug?.Log("Cart {CartId} Created: {MemberNumber}", context.Instance.CorrelationId, context.Data.MemberNumber);
                    })
                    .Schedule(CartTimeout, context => context.Init<CartExpired>(context.Instance),
                        context => TimeSpan.FromSeconds(context.Instance.ExpiresAfterSeconds))
                    .TransitionTo(Active));

                During(Active,
                    When(CartTimeout.Received)
                        .Then(context => LogContext.Debug?.Log("Cart Expired: {MemberNumber}", context.Data.MemberNumber))
                        .PublishAsync(context => context.Init<CartRemoved>(context.Instance))
                        .Finalize(),
                    When(Submitted)
                        .Then(context => LogContext.Debug?.Log("Cart Submitted: {MemberNumber}", context.Data.MemberNumber))
                        .Unschedule(CartTimeout)
                        .PublishAsync(context => context.Init<CartRemoved>(context.Instance))
                        .Finalize(),
                    When(ItemAdded)
                        .Then(context => LogContext.Debug?.Log("Cart Item Added: {MemberNumber}", context.Data.MemberNumber))
                        .Schedule(CartTimeout, context => context.Init<CartExpired>(context.Instance),
                            context => TimeSpan.FromSeconds(context.Instance.ExpiresAfterSeconds)));

                SetCompletedWhenFinalized();
            }

            public Schedule<TestState, CartExpired> CartTimeout { get; private set; }

            public Event<CartItemAdded> ItemAdded { get; private set; }
            public Event<OrderSubmitted> Submitted { get; private set; }

            public State Active { get; private set; }
        }
    }
}
