namespace MassTransit.DapperIntegration.Tests
{
    namespace ContainerTests
    {
        using System;
        using System.Threading.Tasks;
        using Dapper;
        using Dapper.Contrib.Extensions;
        using Microsoft.Data.SqlClient;
        using Microsoft.Extensions.DependencyInjection;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Sagas;
        using Testing;


        public class Using_the_container_integration :
            InMemoryTestFixture
        {
            readonly IServiceProvider _provider;
            string _connectionString;

            public Using_the_container_integration()
            {
                _provider = new ServiceCollection()
                    .AddMassTransit(ConfigureRegistration)
                    .AddScoped<PublishTestStartedActivity>().BuildServiceProvider();
            }

            [Test]
            [Category("Flaky")]
            public async Task Should_work_as_expected()
            {
                Task<ConsumeContext<TestStarted>> started = await ConnectPublishHandler<TestStarted>();
                Task<ConsumeContext<TestUpdated>> updated = await ConnectPublishHandler<TestUpdated>();

                var correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new StartTest
                {
                    CorrelationId = correlationId,
                    TestKey = "Unique"
                });

                await started;

                var repository = _provider.GetRequiredService<ISagaRepository<TestInstance>>();

                var machine = _provider.GetRequiredService<TestStateMachineSaga>();

                Guid? sagaId = await repository.ShouldContainSagaInState(correlationId, machine, x => x.Active, TestTimeout);
                Assert.That(sagaId.HasValue);

                await InputQueueSendEndpoint.Send(new UpdateTest
                {
                    TestId = correlationId,
                    TestKey = "Unique"
                });

                await updated;
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var sql = @"
                if not exists (select * from sysobjects where name='TestInstances' and xtype='U')
                CREATE TABLE TestInstances (
                    CorrelationId uniqueidentifier NOT NULL,
                    CONSTRAINT PK_TestInstances_CorrelationId PRIMARY KEY CLUSTERED (CorrelationId),
                    [Key] nvarchar(max),
                    CurrentState nvarchar(max)
                );
            ";
                    connection.Execute(sql);
                }
            }

            protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
            {
                _connectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();

                configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .DapperRepository(_connectionString);

                configurator.AddBus(provider => BusControl);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.UseInMemoryOutbox();
                configurator.ConfigureSaga<TestInstance>(_provider.GetRequiredService<IBusRegistrationContext>());
            }
        }


        public class TestInstance :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public string Key { get; set; }

            [ExplicitKey]
            public Guid CorrelationId { get; set; }
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
            IStateMachineActivity<TestInstance>
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

            public async Task Execute(BehaviorContext<TestInstance> context, IBehavior<TestInstance> next)
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public async Task Execute<T>(BehaviorContext<TestInstance, T> context, IBehavior<TestInstance, T> next)
                where T : class
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TestInstance, TException> context, IBehavior<TestInstance> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public Task Faulted<T, TException>(BehaviorExceptionContext<TestInstance, T, TException> context, IBehavior<TestInstance, T> next)
                where TException : Exception
                where T : class
            {
                return next.Faulted(context);
            }
        }
    }
}
