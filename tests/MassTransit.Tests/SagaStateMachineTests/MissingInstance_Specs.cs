namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_an_existing_instance_is_not_found :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_the_event_of_the_missing_instance()
        {
            IRequestClient<CheckStatus> requestClient = Bus.CreateRequestClient<CheckStatus>(InputQueueAddress, TestTimeout);
            (Task<Response<Status>> status, Task<Response<InstanceNotFound>> notFound) =
                await requestClient.GetResponse<Status, InstanceNotFound>(new CheckStatus("A"), TestCancellationToken);

            Assert.That(async () => await status, Throws.TypeOf<TaskCanceledException>());

            await notFound;

            Assert.AreEqual("A", notFound.Result.Message.ServiceName);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public string ServiceName { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started, x => x
                    .CorrelateBy(instance => instance.ServiceName, context => context.Message.ServiceName)
                    .SelectId(context => context.Message.ServiceId));

                Event(() => CheckStatus, x =>
                {
                    x.CorrelateBy(instance => instance.ServiceName, context => context.Message.ServiceName);

                    x.OnMissingInstance(m =>
                    {
                        return m.ExecuteAsync(context => context.RespondAsync(new InstanceNotFound(context.Message.ServiceName)));
                    });
                });

                Initially(
                    When(Started)
                        .Then(context => context.Instance.ServiceName = context.Data.ServiceName)
                        .Respond(context => new StartupComplete
                        {
                            ServiceId = context.Instance.CorrelationId,
                            ServiceName = context.Instance.ServiceName
                        })
                        .Then(context => Console.WriteLine("Started: {0} - {1}", context.Instance.CorrelationId, context.Instance.ServiceName))
                        .TransitionTo(Running));

                During(Running,
                    When(CheckStatus)
                        .Then(context => Console.WriteLine("Status check!"))
                        .Respond(context => new Status("Running", context.Instance.ServiceName)));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<CheckStatus> CheckStatus { get; private set; }
        }


        class InstanceNotFound
        {
            public InstanceNotFound(string serviceName)
            {
                ServiceName = serviceName;
            }

            public string ServiceName { get; set; }
        }


        class Status
        {
            public Status(string status, string serviceName)
            {
                StatusDescription = status;
                ServiceName = serviceName;
            }

            public string ServiceName { get; set; }
            public string StatusDescription { get; set; }
        }


        class CheckStatus
        {
            public CheckStatus(string serviceName)
            {
                ServiceName = serviceName;
            }

            public CheckStatus()
            {
            }

            public string ServiceName { get; set; }
        }


        class Start
        {
            public Start(string serviceName, Guid serviceId)
            {
                ServiceName = serviceName;
                ServiceId = serviceId;
            }

            public string ServiceName { get; set; }
            public Guid ServiceId { get; set; }
        }


        class StartupComplete
        {
            public Guid ServiceId { get; set; }
            public string ServiceName { get; set; }
        }
    }
}
