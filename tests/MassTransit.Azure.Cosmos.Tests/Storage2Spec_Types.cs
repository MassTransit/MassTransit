namespace MassTransit.Azure.Cosmos.Tests
{
    using System;


    public class RehersalBegins
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


    public class ChoirStateOptimistic :
        SagaStateMachineInstance
    {
        protected ChoirStateOptimistic()
        {
        }

        public ChoirStateOptimistic(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string CurrentState { get; set; }
        public int Harmony { get; set; }
        public string BassName { get; set; }
        public string BaritoneName { get; set; }
        public string TenorName { get; set; }
        public string CountertenorName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class ChoirStateMachine :
        MassTransitStateMachine<ChoirStateOptimistic>
    {
        public ChoirStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => RehersalStarts, x =>
            {
                x.CorrelateById(context => context.Message.CorrelationId);
            });
            Event(() => BassStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => BaritoneStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => TenorStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => CountertenorStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            CompositeEvent(() => AllSinging, x => x.Harmony, BassStarts, BaritoneStarts, TenorStarts, CountertenorStarts);

            Initially(
                When(RehersalStarts)
                    .Then(context => Console.WriteLine("Rehersal Started!!"))
                    .TransitionTo(Warmup));

            During(Warmup,
                When(BassStarts)
                    .Then(context => Console.WriteLine("Bass Started!!"))
                    .Then(context => context.Instance.BassName = context.Data.Name),
                When(BaritoneStarts)
                    .Then(context => Console.WriteLine("Baritone Started!!"))
                    .Then(context => context.Instance.BaritoneName = context.Data.Name),
                When(TenorStarts)
                    .Then(context => Console.WriteLine("Tenor Started!!"))
                    .Then(context => context.Instance.TenorName = context.Data.Name),
                When(CountertenorStarts)
                    .Then(context => Console.WriteLine("CounterTenor Started!!"))
                    .Then(context => context.Instance.CountertenorName = context.Data.Name),
                When(AllSinging)
                    .Then(context => Console.WriteLine("Harmony Reached!!"))
                    .TransitionTo(Harmony));
        }

        public Event<RehersalBegins> RehersalStarts { get; private set; }
        public Event<Bass> BassStarts { get; private set; }
        public Event<Baritone> BaritoneStarts { get; private set; }
        public Event<Tenor> TenorStarts { get; private set; }
        public Event<Countertenor> CountertenorStarts { get; private set; }
        public Event AllSinging { get; private set; }
        public State Warmup { get; private set; }
        public State Harmony { get; private set; }
    }


    public class ChoirStatePessimistic :
        SagaStateMachineInstance
    {
        protected ChoirStatePessimistic()
        {
        }

        public ChoirStatePessimistic(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string CurrentState { get; set; }
        public int Harmony { get; set; }
        public string BassName { get; set; }
        public string BaritoneName { get; set; }
        public string TenorName { get; set; }
        public string CountertenorName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class ChoirStatePessimisticMachine :
        MassTransitStateMachine<ChoirStatePessimistic>
    {
        public ChoirStatePessimisticMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => RehersalStarts, x =>
            {
                x.CorrelateById(context => context.Message.CorrelationId);
                x.InsertOnInitial = true;
            });
            Event(() => BassStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => BaritoneStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => TenorStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => CountertenorStarts, x => x.CorrelateById(context => context.Message.CorrelationId));
            CompositeEvent(() => AllSinging, x => x.Harmony, BassStarts, BaritoneStarts, TenorStarts, CountertenorStarts);

            Initially(
                When(RehersalStarts)
                    .Then(context => Console.WriteLine("Rehersal Started!!"))
                    .TransitionTo(Warmup));

            During(Warmup,
                When(BassStarts)
                    .Then(context => Console.WriteLine("Bass Started!!"))
                    .Then(context => context.Instance.BassName = context.Data.Name),
                When(BaritoneStarts)
                    .Then(context => Console.WriteLine("Baritone Started!!"))
                    .Then(context => context.Instance.BaritoneName = context.Data.Name),
                When(TenorStarts)
                    .Then(context => Console.WriteLine("Tenor Started!!"))
                    .Then(context => context.Instance.TenorName = context.Data.Name),
                When(CountertenorStarts)
                    .Then(context => Console.WriteLine("CounterTenor Started!!"))
                    .Then(context => context.Instance.CountertenorName = context.Data.Name),
                When(AllSinging)
                    .Then(context => Console.WriteLine("Harmony Reached!!"))
                    .TransitionTo(Harmony));
        }

        public Event<RehersalBegins> RehersalStarts { get; private set; }
        public Event<Bass> BassStarts { get; private set; }
        public Event<Baritone> BaritoneStarts { get; private set; }
        public Event<Tenor> TenorStarts { get; private set; }
        public Event<Countertenor> CountertenorStarts { get; private set; }
        public Event AllSinging { get; private set; }
        public State Warmup { get; private set; }
        public State Harmony { get; private set; }
    }
}
