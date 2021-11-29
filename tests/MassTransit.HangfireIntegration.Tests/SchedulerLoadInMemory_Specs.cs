namespace MassTransit.HangfireIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    [Category("Flaky")]
    public class When_processing_a_lot_of_saga_instances :
        HangfireInMemoryTestFixture
    {
        [Test]
        public async Task Should_remove_the_saga_once_completed()
        {
            var tasks = new List<Task<ConsumeContext<Stopped>>>();

            for (var i = 0; i < 20; i++)
            {
                var correlationId = NewId.NextGuid();

                tasks.Add(await ConnectPublishHandler<Stopped>(context => context.Message.CorrelationId == correlationId));

                await InputQueueSendEndpoint.Send(new Start {CorrelationId = correlationId});
            }

            await Task.WhenAll(tasks.ToArray());

            await Task.Delay(1000);

            Assert.AreEqual(0, _repository.Count);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public When_processing_a_lot_of_saga_instances()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
        }


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

            public Guid? ScheduleTokenId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Schedule(() => StopSchedule, x => x.ScheduleTokenId, x => x.Delay = TimeSpan.FromMilliseconds(100));

                Initially(
                    When(Started)
                        .Schedule(StopSchedule, context => new Stop {CorrelationId = context.Instance.CorrelationId})
                        .Then(context => Console.WriteLine($"Started: {context.Instance.CorrelationId}"))
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Publish(context => new Stopped {CorrelationId = context.Instance.CorrelationId})
                        .Then(context => Console.WriteLine($"Stopped: {context.Instance.CorrelationId}"))
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public Schedule<Instance, Stop> StopSchedule { get; private set; }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        public class Stopped :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
