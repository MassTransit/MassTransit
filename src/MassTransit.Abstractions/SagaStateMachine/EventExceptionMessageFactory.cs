namespace MassTransit
{
    using System;


    /// <summary>
    /// Returns a message from an event exception
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate TMessage EventExceptionMessageFactory<TSaga, in TException, out TMessage>(BehaviorExceptionContext<TSaga, TException> context)
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
    public delegate T EventExceptionMessageFactory<TSaga, in TMessage, in TException, out T>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
        where TSaga : class, ISaga
        where TMessage : class
        where TException : Exception;
}
