namespace Automatonymous
{
    using System;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressExceptionProvider<in TInstance, in TException>(ConsumeExceptionEventContext<TInstance, TException> context)
        where TException : Exception;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressExceptionProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class
        where TException : Exception;
}
