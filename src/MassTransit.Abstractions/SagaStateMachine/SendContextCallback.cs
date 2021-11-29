namespace MassTransit
{
    public delegate void SendContextCallback<TSaga, in T>(BehaviorContext<TSaga> context, SendContext<T> sendContext)
        where TSaga : class, ISaga
        where T : class;


    public delegate void SendContextCallback<TSaga, in TMessage, in T>(BehaviorContext<TSaga, TMessage> context, SendContext<T> sendContext)
        where TSaga : class, ISaga
        where TMessage : class
        where T : class;
}
