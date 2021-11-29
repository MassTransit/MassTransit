namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Returns a message from an event exception
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<T> AsyncEventExceptionMessageFactory<TSaga, in TException, T>(BehaviorExceptionContext<TSaga, TException> context)
        where TSaga : class, ISaga
        where TException : Exception;


    /// <summary>
    /// Returns a message from an event exception
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<T> AsyncEventExceptionMessageFactory<TSaga, in TMessage, in TException, T>(
        BehaviorExceptionContext<TSaga, TMessage, TException> context)
        where TSaga : class, ISaga
        where TMessage : class
        where TException : Exception
        where T : class;
}
