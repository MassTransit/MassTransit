namespace MassTransit
{
    using System;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressProvider<TSaga>(BehaviorContext<TSaga> context)
        where TSaga : class, ISaga;


    /// <summary>
    /// Provides an address for the request service
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Uri ServiceAddressProvider<TSaga, in TMessage>(BehaviorContext<TSaga, TMessage> context)
        where TSaga : class, ISaga
        where TMessage : class;
}
