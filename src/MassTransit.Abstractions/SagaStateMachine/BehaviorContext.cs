namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A behavior context is an event context delivered to a behavior, including the state instance
    /// </summary>
    /// <typeparam name="TSaga">The state instance type</typeparam>
    public interface BehaviorContext<TSaga> :
        SagaConsumeContext<TSaga>
        where TSaga : class, ISaga
    {
        StateMachine<TSaga> StateMachine { get; }

        Event Event { get; }

        [Obsolete("Deprecated, use Saga instead")]
        TSaga Instance { get; }

        /// <summary>
        /// Raise an event on the current instance, pushing the current event on the stack
        /// </summary>
        /// <param name="event">The event to raise</param>
        /// <returns>An awaitable Task</returns>
        Task Raise(Event @event);

        /// <summary>
        /// Raise an event on the current instance, pushing the current event on the stack
        /// </summary>
        /// <param name="event">The event to raise</param>
        /// <param name="data">THe event data</param>
        /// <returns>An awaitable Task</returns>
        Task Raise<T>(Event<T> @event, T data)
            where T : class;

        Task<SendTuple<T>> Init<T>(object values)
            where T : class;

        /// <summary>
        /// Return a proxy of the current behavior context with the specified event
        /// </summary>
        /// <param name="event">The event for the new context</param>
        /// <returns></returns>
        BehaviorContext<TSaga> CreateProxy(Event @event);

        /// <summary>
        /// Return a proxy of the current behavior context with the specified event and data
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="event">The event for the new context</param>
        /// <param name="data">The data for the event</param>
        /// <returns></returns>
        BehaviorContext<TSaga, T> CreateProxy<T>(Event<T> @event, T data)
            where T : class;
    }


    /// <summary>
    /// A behavior context include an event context, along with the behavior for a state instance.
    /// </summary>
    /// <typeparam name="TSaga">The instance type</typeparam>
    /// <typeparam name="TMessage">The event type</typeparam>
    public interface BehaviorContext<TSaga, out TMessage> :
        SagaConsumeContext<TSaga, TMessage>,
        BehaviorContext<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
    {
        new Event<TMessage> Event { get; }

        [Obsolete("Deprecated, use Message instead")]
        TMessage Data { get; }

        new Task<SendTuple<T>> Init<T>(object values)
            where T : class;
    }
}
