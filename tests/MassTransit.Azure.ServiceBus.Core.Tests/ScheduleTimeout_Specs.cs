namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    namespace ScheduleTimeout_Specs
    {
        using System;
        using System.Threading.Tasks;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
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

                Assert.That(saga.HasValue, Is.True);

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

                Assert.That(saga.HasValue, Is.True);

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

                configurator.UseInMemoryOutbox();
                configurator.StateMachineSaga(_machine, _repository);
            }
        }


        [TestFixture]
        public class Using_the_container_with_azure_message_scheduler
        {
            [Test, Explicit]
            public async Task Should_not_interfere_with_message_cancellation()
            {
                await using var provider = new ServiceCollection()
                    .AddScoped<SetScopedValueActivity>()
                    .AddScoped<ScopedService>()
                    .AddTelemetryListener()
                    .AddMassTransitTestHarness(x =>
                    {
                        x.AddHandler(async (CartRemoved _) =>
                        {
                        });
                        x.AddSagaStateMachine<TestStateMachineContainer, TestState>();

                        x.UsingTestAzureServiceBus((context, cfg) =>
                        {
                            cfg.UseMessageScope(context);
                            cfg.UseSendFilter(typeof(SendFilter<>), context);
                            cfg.UseInMemoryOutbox();
                        });
                    }).BuildServiceProvider(true);

                var harness = provider.GetTestHarness();
                harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

                await harness.Start();

                var memberNumber = NewId.NextGuid().ToString();

                await harness.Bus.Publish<CartItemAdded>(new { MemberNumber = memberNumber });

                await Task.Delay(500);

                await harness.Bus.Publish<OrderSubmitted>(new { MemberNumber = memberNumber });

                await Task.Delay(3000);
            }
        }


        public class TestState :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }

            public string MemberNumber { get; set; }

            public Guid? CartTimeoutTokenId { get; set; }

            public int ExpiresAfterSeconds { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public record CartItemAdded
        {
            public string MemberNumber { get; init; }
        }


        public record CartRemoved
        {
            public string MemberNumber { get; init; }
        }


        public record CartExpired
        {
            public string MemberNumber { get; init; }
        }


        public record OrderSubmitted
        {
            public string MemberNumber { get; init; }
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


        class TestStateMachineContainer :
            MassTransitStateMachine<TestState>
        {
            public TestStateMachineContainer()
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
                    .Activity(x => x.OfType<SetScopedValueActivity>())
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


        public class ScopedService
        {
            public string ScopedValue { get; set; }
        }


        public class SetScopedValueActivity :
            IStateMachineActivity<TestState, CartItemAdded>
        {
            readonly ILogger _logger;
            readonly ScopedService _scopedService;

            public SetScopedValueActivity(ScopedService scopedService, ILogger<SetScopedValueActivity> logger)
            {
                _scopedService = scopedService;
                _logger = logger;
            }

            public async Task Execute(BehaviorContext<TestState, CartItemAdded> context, IBehavior<TestState, CartItemAdded> next)
            {
                _logger.LogInformation("Setting scoped value for {CorrelationId}", context.CorrelationId);

                _scopedService.ScopedValue = "Correct scoped value";

                await next.Execute(context);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TestState, CartItemAdded, TException> context, IBehavior<TestState, CartItemAdded> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public void Probe(ProbeContext context)
            {
            }

            public void Accept(StateMachineVisitor visitor)
            {
            }
        }


        public class SendFilter<TMessage> : IFilter<SendContext<TMessage>>
            where TMessage : class
        {
            readonly ILogger _logger;
            readonly ScopedService _scopedService;

            public SendFilter(ScopedService scopedService, ILogger<SendFilter<TMessage>> logger)
            {
                _scopedService = scopedService;
                _logger = logger;
            }

            public Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
            {
                _logger.LogInformation("Scoped value for {Message} is : {Value}", typeof(TMessage), _scopedService.ScopedValue);

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
