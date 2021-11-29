namespace MassTransit.NHibernateIntegration.Tests
{
    namespace ReadOnlyTests
    {
        using System;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class When_a_readonly_event_is_consumed :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_not_update_the_saga_repository()
            {
                var serviceId = NewId.NextGuid();

                IRequestClient<Start> startClient = Bus.CreateRequestClient<Start>(InputQueueAddress, TestTimeout);

                await startClient.GetResponse<StartupComplete>(new Start { CorrelationId = serviceId }, TestCancellationToken);

                IRequestClient<CheckStatus> requestClient = Bus.CreateRequestClient<CheckStatus>(InputQueueAddress, TestTimeout);

                Response<Status> status = await requestClient.GetResponse<Status>(new CheckStatus { CorrelationId = serviceId }, TestCancellationToken);

                Assert.That(status.Message.StatusText, Is.EqualTo("Started"));

                status = await requestClient.GetResponse<Status>(new CheckStatus { CorrelationId = serviceId }, TestCancellationToken);

                Assert.That(status.Message.StatusText, Is.EqualTo("Started"));
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _machine = new ReadOnlyStateMachine();

                configurator.StateMachineSaga(_machine, CreateSagaRepository());
            }

            static ISagaRepository<ReadOnlyInstance> CreateSagaRepository()
            {
                var provider = new SQLiteSessionFactoryProvider(false, typeof(ReadOnlyInstanceMap));
                var sessionFactory = provider.GetSessionFactory();
                ISagaRepository<ReadOnlyInstance> sagaRepository = NHibernateSagaRepository<ReadOnlyInstance>.Create(sessionFactory);
                return sagaRepository;
            }

            ReadOnlyStateMachine _machine;
        }


        public class ReadOnlyInstanceMap :
            SagaClassMapping<ReadOnlyInstance>
        {
            public ReadOnlyInstanceMap()
            {
                Property(x => x.CurrentState);

                Property(x => x.StatusText, x => x.Length(40));
            }
        }


        public class ReadOnlyInstance :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public string StatusText { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class ReadOnlyStateMachine :
            MassTransitStateMachine<ReadOnlyInstance>
        {
            public ReadOnlyStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => StatusCheckRequested, x =>
                {
                    x.ReadOnly = true;
                });

                Initially(
                    When(Started)
                        .Then(context => context.Instance.StatusText = "Started")
                        .Respond(context => new StartupComplete { CorrelationId = context.Instance.CorrelationId })
                        .TransitionTo(Running)
                );

                During(Running,
                    When(StatusCheckRequested)
                        .Respond(context => new Status
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            StatusText = context.Instance.StatusText
                        })
                        .Then(context => context.Instance.StatusText = "Running")
                );
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<CheckStatus> StatusCheckRequested { get; private set; }
        }


        class Status :
            CorrelatedBy<Guid>
        {
            public string StatusText { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class CheckStatus :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class StartupComplete
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
