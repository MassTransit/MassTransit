namespace MassTransit.MartenIntegration.Tests
{
    namespace ContainerTests
    {
        using System;
        using System.Linq;
        using System.Threading;
        using System.Threading.Tasks;
        using Marten;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Hosting;
        using Microsoft.Extensions.Logging;
        using Microsoft.Extensions.Logging.Abstractions;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Sagas;
        using Testing;

        public class Using_Marten_with_multiple_sagas :
            InMemoryTestFixture
        {
            readonly IServiceProvider _provider;

            public Using_Marten_with_multiple_sagas()
            {
                _provider = new ServiceCollection()
                    .AddMassTransit(ConfigureRegistration)
                    .AddScoped<PublishTestStartedActivity>().BuildServiceProvider();
            }

            [Test]
            public async Task Should_work_as_expected()
            {
                await EnsureMartenHostedServiceIsStarted();

                Task<ConsumeContext<TestStarted>> started = await ConnectPublishHandler<TestStarted>();

                var correlationId = NewId.NextGuid();

                await Bus.Publish(new StartTest
                {
                    CorrelationId = correlationId,
                    TestKey = "Unique"
                });

                await started;

                var repository = _provider.GetRequiredService<ISagaRepository<TestInstance>>();

                var machine = _provider.GetRequiredService<TestStateMachineSaga>();

                Guid? sagaId = await repository.ShouldContainSagaInState(correlationId, machine, x => x.Active, TestTimeout);
                Assert.That(sagaId.HasValue);
            }

            protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
            {
                configurator.AddLogging(loggingBuilder => loggingBuilder.AddProvider(NullLoggerProvider.Instance));

                configurator.AddMarten(options =>
                {
                    options.Connection("server=localhost;port=5432;database=MartenTest;user id=postgres;password=Password12!;");
                    options.CreateDatabasesForTenants(c =>
                    {
                        c.MaintenanceDatabase("server=localhost;port=5432;database=postgres;user id=postgres;password=Password12!;");
                        c.ForTenant()
                            .CheckAgainstPgDatabase()
                            .WithOwner("postgres")
                            .WithEncoding("UTF-8")
                            .ConnectionLimit(-1);
                    });
                }).ApplyAllDatabaseChangesOnStartup();

                configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .MartenRepository();
                configurator.AddSagaStateMachine<TestStateMachineSaga2, TestInstance2>()
                    .MartenRepository();

                configurator.AddBus(provider => BusControl);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                var busRegistrationContext = _provider.GetRequiredService<IBusRegistrationContext>();
                configurator.UseInMemoryOutbox(busRegistrationContext);
                configurator.ConfigureSaga<TestInstance>(busRegistrationContext);
                configurator.ConfigureSaga<TestInstance2>(busRegistrationContext);
            }

            private Task EnsureMartenHostedServiceIsStarted() =>
                _provider
                    .GetServices<IHostedService>()
                    .Single(x => x.GetType().Name == "MartenActivator")
                    .StartAsync(CancellationToken.None);
        }


        public class TestInstance2 :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public string Key { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class TestStateMachineSaga2 :
            MassTransitStateMachine<TestInstance2>
        {
            public TestStateMachineSaga2()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .Then(context => context.Instance.Key = context.Data.TestKey)
                        .Activity(x => x.OfInstanceType<PublishTestStartedActivity2>())
                        .TransitionTo(Active));

                SetCompletedWhenFinalized();
            }

            public State Active { get; private set; }
            public State Done { get; private set; }

            public Event<StartTest> Started { get; private set; }
        }


        public class PublishTestStartedActivity2 :
            IStateMachineActivity<TestInstance2>
        {
            readonly ConsumeContext _context;

            public PublishTestStartedActivity2(ConsumeContext context)
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

            public async Task Execute(BehaviorContext<TestInstance2> context, IBehavior<TestInstance2> next)
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public async Task Execute<T>(BehaviorContext<TestInstance2, T> context, IBehavior<TestInstance2, T> next)
                where T : class
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TestInstance2, TException> context, IBehavior<TestInstance2> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public Task Faulted<T, TException>(BehaviorExceptionContext<TestInstance2, T, TException> context, IBehavior<TestInstance2, T> next)
                where TException : Exception
                where T : class
            {
                return next.Faulted(context);
            }
        }
    }
}
