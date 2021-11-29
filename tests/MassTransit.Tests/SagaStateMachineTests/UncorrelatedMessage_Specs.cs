namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_a_message_is_not_correlated :
        InMemoryTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_retry_the_status_message()
        {
            Task<Response<Status>> statusTask = Bus.Request<CheckStatus, Status>(InputQueueAddress, new CheckStatus("A"), TestCancellationToken);

            await InputQueueSendEndpoint.Send(new Start("A", Guid.NewGuid()));

            Response<Status> status = await statusTask;

            Assert.AreEqual("A", status.Message.ServiceName);
        }

        [Test]
        public async Task Should_start_and_handle_the_status_request()
        {
            Response<StartupComplete> startupComplete =
                await Bus.Request<Start, StartupComplete>(InputQueueAddress, new Start("A", Guid.NewGuid()), TestCancellationToken);

            Response<Status> status = await Bus.Request<CheckStatus, Status>(InputQueueAddress, new CheckStatus("A"), TestCancellationToken);

            Assert.AreEqual("A", status.Message.ServiceName);
        }

        [Test]
        public async Task Should_start_and_handle_the_status_request_awaited()
        {
            Response<StartupComplete> startupComplete =
                await Bus.Request<Start, StartupComplete>(InputQueueAddress, new Start("B", Guid.NewGuid()), TestCancellationToken);

            Response<Status> status = await Bus.Request<CheckStatus, Status>(InputQueueAddress, new CheckStatus("B"), TestCancellationToken);

            Assert.AreEqual("B", status.Message.ServiceName);
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

                Event(() => CheckStatus, x => x
                    .CorrelateBy(instance => instance.ServiceName, context => context.Message.ServiceName));

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


        class Status
        {
            public Status()
            {
            }

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

            public Start()
            {
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
