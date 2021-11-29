namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;
    using SagaStateMachine;
    using Visualizer;


    [TestFixture]
    public class When_visualizing_a_state_machine
    {
        [Test]
        public void Should_parse_the_graph()
        {
            Assert.IsNotNull(_graph);
        }

        [Test]
        public void Should_show_the_goods()
        {
            var generator = new StateMachineGraphvizGenerator(_graph);

            string dots = generator.CreateDotFile();

            Console.WriteLine(dots);

            var expected = Expected.Replace("\r", "").Replace("\n", Environment.NewLine);

            Assert.AreEqual(expected, dots);
        }

        InstanceStateMachine _machine;
        StateMachineGraph _graph;

        [OneTimeSetUp]
        public void Setup()
        {
            _machine = new InstanceStateMachine();

            _graph = _machine.GetGraph();
        }

        const string Expected = @"digraph G {
0 [shape=ellipse, label=""Initial""];
1 [shape=ellipse, label=""Running""];
2 [shape=ellipse, label=""Failed""];
3 [shape=ellipse, label=""Final""];
4 [shape=ellipse, label=""Suspended""];
5 [shape=rectangle, label=""Initialized""];
6 [shape=rectangle, label=""Exception""];
7 [shape=rectangle, label=""Finished""];
8 [shape=rectangle, label=""Suspend""];
9 [shape=rectangle, label=""Resume""];
10 [shape=rectangle, label=""Restart<RestartData>""];
0 -> 5;
1 -> 7;
1 -> 8;
2 -> 10;
4 -> 9;
5 -> 1;
5 -> 6;
6 -> 2;
7 -> 3;
8 -> 4;
9 -> 1;
10 -> 1;
}";


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running)
                        .Catch<Exception>(h => h.TransitionTo(Failed)));

                During(Running,
                    When(Finished)
                        .TransitionTo(Final),
                    When(Suspend)
                        .TransitionTo(Suspended),
                    Ignore(Resume));

                During(Suspended,
                    When(Resume)
                        .TransitionTo(Running));

                During(Failed,
                    When(Restart, context => context.Data.Name != null)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public State Suspended { get; private set; }
            public State Failed { get; private set; }

            public Event Initialized { get; private set; }
            public Event Suspend { get; private set; }
            public Event Resume { get; private set; }
            public Event Finished { get; private set; }

            public Event<RestartData> Restart { get; private set; }
        }


        class RestartData
        {
            public string Name { get; set; }
        }
    }
}
