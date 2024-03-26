namespace MassTransit
{
}


namespace Automatonymous
{
    using System;
    using MassTransit;


    /// <summary>
    /// A behavior is a chain of activities invoked by a state
    /// </summary>
    /// <typeparam name="TSaga">The state type</typeparam>
    [Obsolete("Deprecated, use IBehavior instead")]
    public interface Behavior<TSaga> :
        IBehavior<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
    }


    /// <summary>
    /// A behavior is a chain of activities invoked by a state
    /// </summary>
    /// <typeparam name="TSaga">The state type</typeparam>
    /// <typeparam name="TMessage">The data type of the behavior</typeparam>
    [Obsolete("Deprecated, use IBehavior instead")]
    public interface Behavior<TSaga, in TMessage> :
        IBehavior<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
    }
}
