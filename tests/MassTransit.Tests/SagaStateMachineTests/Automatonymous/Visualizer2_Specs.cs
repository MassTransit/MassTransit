namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;
    using SagaStateMachine;
    using Visualizer;
    using Visualizer2;


    namespace Visualizer2
    {
        using System;


        public class IngestState :
            SagaStateMachineInstance,
            ISagaVersion
        {
            public Guid OrderId { get; set; }

            public Guid? InventoryRequestId { get; set; }

            public DateTime? OrderReceivedAt { get; set; }

            public string CurrentState { get; set; }

            public int Version { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public class SampleStateMachine :
            MassTransitStateMachine<IngestState>
        {
            public SampleStateMachine()
            {
                InstanceState(x => x.CurrentState);
                Request(() => ProcessGetInventory, x => x.InventoryRequestId);

                Event(() => SubmitOrderEvent, e =>
                {
                    e.CorrelateById(context => context.Message.OrderId);

                    e.InsertOnInitial = true;
                    e.SetSagaFactory(context => new IngestState()
                    {
                        OrderId = context.Message.OrderId,
                        OrderReceivedAt = DateTime.UtcNow
                    });
                });

                Initially(
                    When(SubmitOrderEvent)
                        .Request(ProcessGetInventory, context => context.Init<ReadInventory>(new { Id = context.Saga.OrderId }))
                        .TransitionTo(ProcessGetInventory.Pending)
                );

                During(ProcessGetInventory.Pending,
                    When(ProcessGetInventory.Completed)
                        .TransitionTo(Completed),
                    When(ProcessGetInventory.TimeoutExpired)
                        .TransitionTo(TimeOutError),
                    When(ProcessGetInventory.Faulted)
                        .TransitionTo(FatalError));
            }

            public State Completed { get; set; }

            public State FatalError { get; set; }

            public State TimeOutError { get; set; }

            public Event<SubmitOrder> SubmitOrderEvent { get; set; }

            public Request<IngestState, ReadInventory, ReadInventoryResponse> ProcessGetInventory { get; set; }
        }


        public class SubmitOrder
        {
            public Guid OrderId { get; set; }
        }


        public class ReadInventory
        {
            public Guid Id { get; set; }
        }


        public class ReadInventoryResponse
        {
            public bool IsSuccess { get; set; }
        }
    }


    [TestFixture]
    public class When_visualizing_a_state_machine_again
    {
        [Test]
        public void Should_show_the_goods()
        {
            var machine = new SampleStateMachine();

            var graph = machine.GetGraph();

            var generator = new StateMachineGraphvizGenerator(graph);

            string dots = generator.CreateDotFile();

            Console.WriteLine(dots);
        }
    }
}
