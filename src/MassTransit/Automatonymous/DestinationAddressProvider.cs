namespace Automatonymous
{
    using System;


    /// <summary>
    /// Returns the address for the message provided
    /// </summary>
    /// <typeparam name="TInstance">The saga instance</typeparam>
    /// <typeparam name="TData">The message data</typeparam>
    /// <returns></returns>
    public delegate Uri DestinationAddressProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TInstance : SagaStateMachineInstance
        where TData : class;

    /// <summary>
    /// Returns the address for the message provided
    /// </summary>
    /// <typeparam name="TInstance">The saga instance</typeparam>
    /// <typeparam name="TData">The message data</typeparam>
    /// <returns></returns>
    public delegate Uri ObsoleteDestinationAddressProvider<in TInstance, in TData>(TInstance instance, TData data)
        where TInstance : SagaStateMachineInstance
        where TData : class;


    /// <summary>
    /// Returns the address for the message provided
    /// </summary>
    /// <typeparam name="TInstance">The saga instance</typeparam>
    /// <returns></returns>
    public delegate Uri DestinationAddressProvider<in TInstance>(ConsumeEventContext<TInstance> context)
        where TInstance : SagaStateMachineInstance;
}