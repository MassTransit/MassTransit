namespace MassTransit
{
    public delegate T EventMessageFactory<TSaga, out T>(BehaviorContext<TSaga> context)
        where TSaga : class, SagaStateMachineInstance
        where T : class;


    public delegate T EventMessageFactory<TInstance, in TMessage, out T>(BehaviorContext<TInstance, TMessage> context)
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
        where T : class;
}
