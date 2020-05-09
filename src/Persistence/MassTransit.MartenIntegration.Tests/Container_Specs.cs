namespace MassTransit.MartenIntegration.Tests
{
    namespace ContainerTests
    {
        using System;
        using System.Threading.Tasks;
        using Automatonymous;
        using GreenPipes;
        using Microsoft.Extensions.DependencyInjection;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Sagas;


        public class Using_the_container_integration :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_work_as_expected()
            {
                Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
                Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

                var correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new StartTest
                {
                    CorrelationId = correlationId,
                    TestKey = "Unique"
                });

                await started;

                await InputQueueSendEndpoint.Send(new UpdateTest
                {
                    TestId = correlationId,
                    TestKey = "Unique"
                });

                await updated;
            }

            readonly IServiceProvider _provider;

            public Using_the_container_integration()
            {
                _provider = new ServiceCollection()
                    .AddMassTransit(ConfigureRegistration)
                    .AddScoped<PublishTestStartedActivity>().BuildServiceProvider();
            }

            protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
                where T : class
            {
                configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .MartenRepository("server=localhost;port=5432;database=MartenTest;user id=postgres;password=Password12!;");

                configurator.AddBus(provider => BusControl);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.UseInMemoryOutbox();
                configurator.ConfigureSaga<TestInstance>(_provider);
            }
        }


        public class TestInstance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }

            public string CurrentState { get; set; }
            public string Key { get; set; }
        }


        public class TestStateMachineSaga :
            MassTransitStateMachine<TestInstance>
        {
            public TestStateMachineSaga()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Updated, x => x.CorrelateById(m => m.Message.TestId));

                Initially(
                    When(Started)
                        .Then(context => context.Instance.Key = context.Data.TestKey)
                        .Activity(x => x.OfInstanceType<PublishTestStartedActivity>())
                        .TransitionTo(Active));

                During(Active,
                    When(Updated)
                        .Publish(context => new TestUpdated
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            TestKey = context.Instance.Key
                        })
                        .TransitionTo(Done)
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public State Active { get; private set; }
            public State Done { get; private set; }

            public Event<StartTest> Started { get; private set; }
            public Event<UpdateTest> Updated { get; private set; }
        }


        public class UpdateTest
        {
            public Guid TestId { get; set; }
            public string TestKey { get; set; }
        }


        public class PublishTestStartedActivity :
            Activity<TestInstance>
        {
            readonly ConsumeContext _context;

            public PublishTestStartedActivity(ConsumeContext context)
            {
                _context = context;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("publisher");
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public async Task Execute(BehaviorContext<TestInstance> context, Behavior<TestInstance> next)
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public async Task Execute<T>(BehaviorContext<TestInstance, T> context, Behavior<TestInstance, T> next)
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TestInstance, TException> context, Behavior<TestInstance> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public Task Faulted<T, TException>(BehaviorExceptionContext<TestInstance, T, TException> context, Behavior<TestInstance, T> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }
        }
    }
}
