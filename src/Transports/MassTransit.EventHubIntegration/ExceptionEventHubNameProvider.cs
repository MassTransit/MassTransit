namespace MassTransit
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
    public delegate string ExceptionEventHubNameProvider<TInstance, in TData, in TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
        where TException : Exception
        where TData : class
        where TInstance : class, ISaga;


    /// <summary>
    /// Return the name of the event hub
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate string ExceptionEventHubNameProvider<TInstance, in TException>(BehaviorExceptionContext<TInstance, TException> context)
        where TException : Exception
        where TInstance : class, ISaga;
}
