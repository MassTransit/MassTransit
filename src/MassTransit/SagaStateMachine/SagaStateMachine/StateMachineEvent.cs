namespace MassTransit.SagaStateMachine
{
    public class StateMachineEvent<TInstance>
    {
        public StateMachineEvent(Event @event, bool isTransitionEvent, Event[] events = null, ICompositeEventStatusAccessor<TInstance> accessor = null)
        {
            Event = @event;
            Events = events;
            Accessor = accessor;
            IsTransitionEvent = isTransitionEvent;
        }

        public bool IsTransitionEvent { get; }
        public Event Event { get; internal set; }

        public Event[] Events { get; }
        public ICompositeEventStatusAccessor<TInstance> Accessor { get; }
    }
}
