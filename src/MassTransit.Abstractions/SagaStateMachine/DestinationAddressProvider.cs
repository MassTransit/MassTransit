namespace MassTransit
{
    using System;


    /// <summary>
    /// Returns the address for the message provided
    /// </summary>
    /// <typeparam name="TSaga">The saga instance</typeparam>
    /// <typeparam name="TMessage">The message data</typeparam>
    /// <returns></returns>
    public delegate Uri DestinationAddressProvider<TSaga, in TMessage>(BehaviorContext<TSaga, TMessage> context)
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class;


    /// <summary>
    /// Returns the address for the message provided
    /// </summary>
    /// <typeparam name="TSaga">The saga instance</typeparam>
    /// <returns></returns>
    public delegate Uri DestinationAddressProvider<TSaga>(BehaviorContext<TSaga> context)
        where TSaga : class, SagaStateMachineInstance;
}
