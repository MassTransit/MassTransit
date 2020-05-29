namespace Automatonymous
{
    using System;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressProvider<in TInstance>(ConsumeEventContext<TInstance> context);


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;
}
