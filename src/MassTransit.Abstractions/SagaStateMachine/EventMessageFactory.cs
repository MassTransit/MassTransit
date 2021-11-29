namespace MassTransit
{
    public delegate T EventMessageFactory<TSaga, out T>(BehaviorContext<TSaga> context)
        where TSaga : class, ISaga
        where T : class;


    public delegate T EventMessageFactory<TInstance, in TMessage, out T>(BehaviorContext<TInstance, TMessage> context)
        where TInstance : class, ISaga
        where TMessage : class
        where T : class;
}
