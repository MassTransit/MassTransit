namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IStateMachineActivity :
        IVisitable
    {
    }


    /// <summary>
    /// An activity is part of a behavior that is executed in order
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface IStateMachineActivity<TSaga> :
        IStateMachineActivity
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TSaga> context, IBehavior<TSaga> next);

        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute<T>(BehaviorContext<TSaga, T> context, IBehavior<TSaga, T> next)
            where T : class;

        /// <summary>
        /// The exception path through the behavior allows activities to catch and handle exceptions
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context, IBehavior<TSaga> next)
            where TException : Exception;

        /// <summary>
        /// The exception path through the behavior allows activities to catch and handle exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context, IBehavior<TSaga, T> next)
            where TException : Exception
            where T : class;
    }


    /// <summary>
    /// An activity is part of a behavior that is executed in order
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface IStateMachineActivity<TSaga, TMessage> :
        IStateMachineActivity
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Execute the activity with the given behavior context
        /// </summary>
        /// <param name="context">The behavior context</param>
        /// <param name="next">The behavior that follows this activity</param>
        /// <returns>An awaitable task</returns>
        Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next);

        /// <summary>
        /// The exception path through the behavior allows activities to catch and handle exceptions
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception;
    }
}
