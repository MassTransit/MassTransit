namespace MassTransit.SagaStateMachine
{
    public interface IActivityBinder<TSaga>
        where TSaga : class, ISaga
    {
        Event Event { get; }

        /// <summary>
        /// Returns True if the event is a state transition event (enter/leave/afterLeave/beforeEnter)
        /// for the specified state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool IsStateTransitionEvent(State state);

        /// <summary>
        /// Binds the activity to the state, may also just ignore the event if it's an ignore event
        /// </summary>
        /// <param name="state"></param>
        void Bind(State<TSaga> state);

        /// <summary>
        /// Bind the activities to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Bind(IBehaviorBuilder<TSaga> builder);
    }
}
