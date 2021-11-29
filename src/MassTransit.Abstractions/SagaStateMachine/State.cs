namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface State :
        IVisitable,
        IComparable<State>
    {
        string Name { get; }

        /// <summary>
        /// Raised when the state is entered
        /// </summary>
        Event Enter { get; }

        /// <summary>
        /// Raised when the state is about to be left
        /// </summary>
        Event Leave { get; }

        /// <summary>
        /// Raised just before the state is about to change to a new state
        /// </summary>
        Event<State> BeforeEnter { get; }

        /// <summary>
        /// Raised just after the state has been left and a new state is selected
        /// </summary>
        Event<State> AfterLeave { get; }
    }


    /// <summary>
    /// A state within a state machine that can be targeted with events
    /// </summary>
    /// <typeparam name="TSaga">The instance type to which the state applies</typeparam>
    public interface State<TSaga> :
        State
        where TSaga : class, ISaga
    {
        IEnumerable<Event> Events { get; }

        /// <summary>
        /// Returns the superState of the state, if there is one
        /// </summary>
        State<TSaga> SuperState { get; }

        Task Raise(BehaviorContext<TSaga> context);

        /// <summary>
        /// Raise an event to the state, passing the instance
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task Raise<T>(BehaviorContext<TSaga, T> context)
            where T : class;

        /// <summary>
        /// Bind an activity to an event
        /// </summary>
        /// <param name="event"></param>
        /// <param name="activity"></param>
        void Bind(Event @event, IStateMachineActivity<TSaga> activity);

        /// <summary>
        /// Ignore the specified event in this state. Prevents an exception from being thrown if
        /// the event is raised during this state.
        /// </summary>
        /// <param name="event"></param>
        void Ignore(Event @event);

        /// <summary>
        /// Ignore the specified event in this state if the filter condition passed. Prevents exceptions
        /// from being thrown if the event is raised during this state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="filter"></param>
        void Ignore<T>(Event<T> @event, StateMachineCondition<TSaga, T> filter)
            where T : class;

        /// <summary>
        /// Adds a substate to the state
        /// </summary>
        /// <param name="subState"></param>
        void AddSubstate(State<TSaga> subState);

        /// <summary>
        /// True if the specified state is included in the state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool HasState(State<TSaga> state);

        /// <summary>
        /// True if the specified state is a substate of the current state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool IsStateOf(State<TSaga> state);
    }
}
