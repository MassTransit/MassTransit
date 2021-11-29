namespace MassTransit
{
    using System;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressExceptionProvider<TSaga, in TException>(BehaviorExceptionContext<TSaga, TException> context)
        where TSaga : class, ISaga
        where TException : Exception;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressExceptionProvider<TSaga, in TMessage, in TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context)
        where TSaga : class, ISaga
        where TMessage : class
        where TException : Exception;
}
