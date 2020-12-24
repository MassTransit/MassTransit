namespace Automatonymous
{
    using System;


    /// <summary>
    /// Return the name of the event hub
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate string ExceptionEventHubNameProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TException : Exception
        where TData : class;


    /// <summary>
    /// Return the name of the event hub
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate string ExceptionEventHubNameProvider<in TInstance, in TException>(ConsumeExceptionEventContext<TInstance, TException> context)
        where TException : Exception;
}
