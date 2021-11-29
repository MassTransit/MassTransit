namespace MassTransit
{
    using System;


    /// <summary>
    /// An exceptional behavior context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public interface BehaviorExceptionContext<TSaga, out TException> :
        BehaviorContext<TSaga>
        where TException : Exception
        where TSaga : class, ISaga
    {
        TException Exception { get; }

        /// <summary>
        /// Return a proxy of the current behavior context with the specified event and data
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="event">The event for the new context</param>
        /// <param name="data">The data for the event</param>
        /// <returns></returns>
        new BehaviorExceptionContext<TSaga, T, TException> CreateProxy<T>(Event<T> @event, T data)
            where T : class;
    }


    /// <summary>
    /// An exceptional behavior context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TException"></typeparam>
    public interface BehaviorExceptionContext<TSaga, out TMessage, out TException> :
        BehaviorContext<TSaga, TMessage>,
        BehaviorExceptionContext<TSaga, TException>
        where TException : Exception
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Return a proxy of the current behavior context with the specified event and data
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="event">The event for the new context</param>
        /// <param name="data">The data for the event</param>
        /// <returns></returns>
        new BehaviorExceptionContext<TSaga, T, TException> CreateProxy<T>(Event<T> @event, T data)
            where T : class;
    }
}
