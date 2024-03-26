namespace MassTransit
{
    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        class StateMachineEvent
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
}
