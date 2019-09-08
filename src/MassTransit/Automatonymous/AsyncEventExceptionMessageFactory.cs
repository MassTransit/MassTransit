namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Returns a message from an event exception
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<TMessage> AsyncEventExceptionMessageFactory<in TInstance, in TException, TMessage>(
        ConsumeExceptionEventContext<TInstance, TException> context)
        where TException : Exception;


    /// <summary>
    /// Returns a message from an event exception
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<TMessage> AsyncEventExceptionMessageFactory<in TInstance, in TData, in TException, TMessage>(
        ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class
        where TException : Exception;
}
