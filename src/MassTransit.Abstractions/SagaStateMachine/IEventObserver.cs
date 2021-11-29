namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IEventObserver<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Called before the event context is delivered to the activities
        /// </summary>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PreExecute(BehaviorContext<TSaga> context);

        /// <summary>
        /// Called before the event context is delivered to the activities
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PreExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class;

        /// <summary>
        /// Called when the event has been processed by the activities
        /// </summary>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PostExecute(BehaviorContext<TSaga> context);

        /// <summary>
        /// Called when the event has been processed by the activities
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <param name="context">The event context</param>
        /// <returns></returns>
        Task PostExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class;

        /// <summary>
        /// Called when the activity execution faults and is not handled by the activities
        /// </summary>
        /// <param name="context">The event context</param>
        /// <param name="exception">The exception that was thrown</param>
        /// <returns></returns>
        Task ExecuteFault(BehaviorContext<TSaga> context, Exception exception);

        /// <summary>
        /// Called when the activity execution faults and is not handled by the activities
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The event context</param>
        /// <param name="exception">The exception that was thrown</param>
        /// <returns></returns>
        Task ExecuteFault<T>(BehaviorContext<TSaga, T> context, Exception exception)
            where T : class;
    }
}
