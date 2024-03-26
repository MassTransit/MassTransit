namespace MassTransit
{
    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineCondition<TSaga>(BehaviorContext<TSaga> context)
        where TSaga : class, SagaStateMachineInstance;


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineCondition<TSaga, in TMessage>(BehaviorContext<TSaga, TMessage> context)
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class;
}
