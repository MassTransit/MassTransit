namespace MassTransit.SagaStateMachine
{
    public class StateMachineEvent<TInstance>
    {
        public StateMachineEvent(Event @event, bool isTransitionEvent)
        {
            Event = @event;
            IsTransitionEvent = isTransitionEvent;
        }

        public bool IsTransitionEvent { get; }
        public Event Event { get; }
    }
}
