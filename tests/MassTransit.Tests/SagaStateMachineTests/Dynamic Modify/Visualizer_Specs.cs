namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;
    using SagaStateMachine;
    using Visualizer;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_visualizing_a_state_machine
    {
        [Test]
        public void Should_parse_the_graph()
        {
            Assert.That(_graph, Is.Not.Null);
        }

        [Test]
        public void Should_show_graphviz_output()
        {
            var generator = new StateMachineGraphvizGenerator(_graph);

            string output = generator.CreateDotFile();

            Console.WriteLine(output);

            var expected = ExpectedGraphvizFile.Replace("\r", "").Replace("\n", Environment.NewLine);

            Assert.That(output, Is.EqualTo(expected));
        }

        [Test]
        public void Should_show_mermaid_output()
        {
            var generator = new StateMachineMermaidGenerator(_graph);

            string output = generator.CreateMermaidFile();

            Console.WriteLine(output);

            var expected = ExpectedMermaidFile.Replace("\r", "").Replace("\n", Environment.NewLine);

            Assert.That(output, Is.EqualTo(expected));
        }

        StateMachine<Instance> _machine;
        StateMachineGraph _graph;

        [OneTimeSetUp]
        public void Setup()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(b => b
                    .State("Running", out State Running)
                    .State("Suspended", out State Suspended)
                    .State("Failed", out State Failed)
                    .Event("Initialized", out Event Initialized)
                    .Event("Suspend", out Event Suspend)
                    .Event("Resume", out Event Resume)
                    .Event("Finished", out Event Finished)
                    .Event<RestartData>("Restart", out Event<RestartData> Restart)
                    .During(b.Initial)
                    .When(Initialized, (binder) => binder
                        .TransitionTo(Running)
                        .Catch<Exception>(h => h.TransitionTo(Failed))
                    )
                    .During(Running)
                    .When(Finished, (binder) => binder.TransitionTo(b.Final))
                    .When(Suspend, (binder) => binder.TransitionTo(Suspended))
                    .Ignore(Resume)
                    .During(Suspended)
                    .When(Resume, b => b.TransitionTo(Running))
                    .During(Failed)
                    .When(Restart, context => context.Data.Name != null, b => b.TransitionTo(Running))
                );

            _graph = _machine.GetGraph();
        }

        const string ExpectedGraphvizFile = @"digraph G {
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

        const string ExpectedMermaidFile = @"flowchart TB;
    0([""Initial""]) --> 5[""Initialized""];
    1([""Running""]) --> 7[""Finished""];
    1([""Running""]) --> 8[""Suspend""];
    2([""Failed""]) --> 10[""Restart«RestartData»""];
    4([""Suspended""]) --> 9[""Resume""];
    5[""Initialized""] --> 1([""Running""]);
    5[""Initialized""] --> 6[""Exception""];
    6[""Exception""] --> 2([""Failed""]);
    7[""Finished""] --> 3([""Final""]);
    8[""Suspend""] --> 4([""Suspended""]);
    9[""Resume""] --> 1([""Running""]);
    10[""Restart«RestartData»""] --> 1([""Running""]);";


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class RestartData
        {
            public string Name { get; set; }
        }
    }
}
