namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_an_existing_instance_is_not_found_and_message_is_redelivered :
        QuartzInMemoryTestFixture
    {
        [Test]
        public async Task Should_schedule_the_message_and_redeliver_to_the_instance()
        {
            IRequestClient<CheckStatus> requestClient = Bus.CreateRequestClient<CheckStatus>(InputQueueAddress, TestTimeout);
            Task<Response<Status, InstanceNotFound>> response =
                requestClient.GetResponse<Status, InstanceNotFound>(new CheckStatus("A"), TestCancellationToken);

            await Task.Delay(500);

            var message = new Start("A", NewId.NextGuid());

            await InputQueueSendEndpoint.Send(message);

            Response<Status, InstanceNotFound> result = await response;

            Assert.That(result.Is(out Response<InstanceNotFound> _), Is.False);
            Assert.That(result.Is(out Response<Status> status), Is.True);

            Assert.AreEqual("A", status.Message.ServiceName);

            (Task<Response<Status>> statusTask, Task<Response<InstanceNotFound>> notFoundTask) = result;

            Assert.That(async () => await notFoundTask, Throws.TypeOf<TaskCanceledException>());
            Assert.That(async () => await statusTask, Throws.Nothing);
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

                    x.OnMissingInstance(m => m.Redeliver(r =>
                    {
                        r.Interval(5, 1000);
                        r.OnRedeliveryLimitReached(n => n.ExecuteAsync(context => context.RespondAsync(new InstanceNotFound(context.Message.ServiceName))));
                    }));
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
            public Status(string statusDescription, string serviceName)
            {
                StatusDescription = statusDescription;
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
