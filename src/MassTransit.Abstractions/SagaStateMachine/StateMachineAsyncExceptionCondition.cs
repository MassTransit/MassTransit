namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<bool> StateMachineAsyncExceptionCondition<TSaga, in TException>(BehaviorExceptionContext<TSaga, TException> context)
        where TException : Exception
        where TSaga : class, ISaga;


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<bool> StateMachineAsyncExceptionCondition<TSaga, in TMessage, in TException>(
        BehaviorExceptionContext<TSaga, TMessage, TException> context)
        where TException : Exception
        where TSaga : class, ISaga
        where TMessage : class;
}
