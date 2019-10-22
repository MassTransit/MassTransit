namespace Automatonymous
{
    using System;

    /// <summary>
    /// Returns a message from an event exception
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate TMessage EventExceptionMessageFactory<in TInstance, in TException, out TMessage>(
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
    public delegate TMessage EventExceptionMessageFactory<in TInstance, in TData, in TException, out TMessage>(
        ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class
        where TException : Exception;
}
