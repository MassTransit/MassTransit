namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    class StateChangeObserver<T> :
        IStateObserver<T>
        where T : class, ISaga
    {
        public StateChangeObserver()
        {
            Events = new List<StateChange>();
        }

        public IList<StateChange> Events { get; }

        public Task StateChanged(BehaviorContext<T> context, State currentState, State previousState)
        {
            Events.Add(new StateChange(context, currentState, previousState));

            return Task.CompletedTask;
        }


        public struct StateChange
        {
            public BehaviorContext<T> Context;
            public readonly State Current;
            public readonly State Previous;

            public StateChange(BehaviorContext<T> context, State current, State previous)
            {
                Context = context;
                Current = current;
                Previous = previous;
            }
        }
    }
}
