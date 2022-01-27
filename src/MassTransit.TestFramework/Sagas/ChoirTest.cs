namespace MassTransit.TestFramework.Sagas
{
    namespace ChoirConcurrency
    {
        using System;


        public class RehearsalBegins
        {
            public Guid CorrelationId { get; set; }
        }


        public class Bass
        {
            public Guid CorrelationId { get; set; }
            public string Name { get; set; }
        }


        public class Baritone
        {
            public Guid CorrelationId { get; set; }
            public string Name { get; set; }
        }


        public class Tenor
        {
            public Guid CorrelationId { get; set; }
            public string Name { get; set; }
        }


        public class Countertenor
        {
            public Guid CorrelationId { get; set; }
            public string Name { get; set; }
        }


        public class ChoirState :
            SagaStateMachineInstance,
            ISagaVersion
        {
            public string CurrentState { get; set; }
            public int Harmony { get; set; }

            public string BassName { get; set; }
            public string BaritoneName { get; set; }
            public string TenorName { get; set; }
            public string CountertenorName { get; set; }
            public int Version { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public class ChoirStateMachine :
            MassTransitStateMachine<ChoirState>
        {
            public ChoirStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => RehearsalStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
                Event(() => BassStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
                Event(() => BaritoneStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
                Event(() => TenorStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
                Event(() => CountertenorStarts, x => x.CorrelateById(context => context.Message.CorrelationId));

                CompositeEvent(() => AllSinging, x => x.Harmony, BassStarts, BaritoneStarts, TenorStarts, CountertenorStarts);

                Initially(
                    When(RehearsalStarts)
                        .Then(context => Console.WriteLine("Rehearsal Started!!"))
                        .TransitionTo(Warmup));

                During(Warmup,
                    When(BassStarts)
                        .Then(context => Console.WriteLine("Bass Started!!"))
                        .Then(context => context.Saga.BassName = context.Message.Name),
                    When(BaritoneStarts)
                        .Then(context => Console.WriteLine("Baritone Started!!"))
                        .Then(context => context.Saga.BaritoneName = context.Message.Name),
                    When(TenorStarts)
                        .Then(context => Console.WriteLine("Tenor Started!!"))
                        .Then(context => context.Saga.TenorName = context.Message.Name),
                    When(CountertenorStarts)
                        .Then(context => Console.WriteLine("CounterTenor Started!!"))
                        .Then(context => context.Saga.CountertenorName = context.Message.Name),
                    When(AllSinging)
                        .Then(context => Console.WriteLine("Harmony Reached!!"))
                        .TransitionTo(Harmony));
            }

            //
            // ReSharper disable UnassignedGetOnlyAutoProperty
            // ReSharper disable MemberCanBePrivate.Global
            public Event<RehearsalBegins> RehearsalStarts { get; }
            public Event<Bass> BassStarts { get; }
            public Event<Baritone> BaritoneStarts { get; }
            public Event<Tenor> TenorStarts { get; }
            public Event<Countertenor> CountertenorStarts { get; }
            public Event AllSinging { get; }
            public State Warmup { get; }
            public State Harmony { get; }
        }
    }
}
