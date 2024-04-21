namespace MassTransit.Tests.ContainerTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;


    [TestFixture]
    public class When_the_scheduler_is_used_with_a_scoped_filter
    {
        [Test]
        public async Task Should_use_same_scope_for_send()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddDelayedMessageScheduler();

                    x.AddSagaStateMachine<TestStateMachine, TestSaga>()
                        .InMemoryRepository();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseDelayedMessageScheduler();
                        cfg.UseSendFilter(typeof(SendFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .AddScoped<ScopedService>()
                .AddScoped(typeof(SendFilter<>))
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish<InitiateEvent>(new { InVar.CorrelationId });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Consumed.Any<InitiateEvent>(), Is.True);
                Assert.That(await harness.Sent.Any<OutgoingEvent>(), Is.True);
            });

            ISentMessage<ExpiredEvent> message = await harness.Sent.SelectAsync<ExpiredEvent>().FirstOrDefault();
            Assert.That(message, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(message.Context.Headers.TryGetHeader("Scoped-Value", out var value), Is.True);
                Assert.That(value, Is.EqualTo("Correct scoped value"));
            });
        }


        public class TestStateMachine :
            MassTransitStateMachine<TestSaga>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState, Created);

                Event(() => InitiateEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
                Schedule(() => ExpiredTimeout, x => x.ExpiredTimeoutToken, x => x.Received = r => r.CorrelateById(s => s.Message.CorrelationId));

                Initially(
                    When(InitiateEvent)
                        .Activity(selector => selector.OfType<SetScopedValueActivity>())
                        .SendAsync(_ => new Uri("queue:another-queue"), x => x.Init<OutgoingEvent>(x.Saga))
                        .Schedule(ExpiredTimeout, x => x.Init<ExpiredEvent>(x.Saga), _ => TimeSpan.FromSeconds(10))
                        .Finalize()
                );

                SetCompletedWhenFinalized();
            }

            public State Created { get; private set; }

            public Event<InitiateEvent> InitiateEvent { get; private set; }
            public Schedule<TestSaga, ExpiredEvent> ExpiredTimeout { get; set; } = null!;
        }


        public class SendFilter<TMessage> :
            IFilter<SendContext<TMessage>>
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

                context.Headers.Set("Scoped-Value", _scopedService.ScopedValue ?? "NULL");

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        public class ScopedService
        {
            public string ScopedValue { get; set; }
        }


        public class SetScopedValueActivity :
            IStateMachineActivity<TestSaga, InitiateEvent>
        {
            readonly ILogger _logger;
            readonly ScopedService _scopedService;

            public SetScopedValueActivity(ScopedService scopedService, ILogger<SetScopedValueActivity> logger)
            {
                _scopedService = scopedService;
                _logger = logger;
            }

            public async Task Execute(BehaviorContext<TestSaga, InitiateEvent> context, IBehavior<TestSaga, InitiateEvent> next)
            {
                _logger.LogInformation("Setting scoped value for {CorrelationId}", context.CorrelationId);

                _scopedService.ScopedValue = "Correct scoped value";

                await next.Execute(context);
            }

            public void Probe(ProbeContext context)
            {
            }

            public void Accept(StateMachineVisitor visitor)
            {
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TestSaga, InitiateEvent, TException> context, IBehavior<TestSaga, InitiateEvent> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }
        }


        public class TestSaga :
            SagaStateMachineInstance
        {
            public int CurrentState { get; set; }
            public Guid? ExpiredTimeoutToken { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public interface ExpiredEvent :
            CorrelatedBy<Guid>
        {
        }


        public interface InitiateEvent :
            CorrelatedBy<Guid>
        {
        }


        public interface OutgoingEvent :
            CorrelatedBy<Guid>
        {
        }
    }
}
