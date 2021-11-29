namespace MassTransit
{
    using System.Threading.Tasks;


    public delegate Task<T> AsyncEventMessageFactory<TSaga, in TMessage, T>(BehaviorContext<TSaga, TMessage> context)
        where TSaga : class, ISaga
        where TMessage : class
        where T : class;


    public delegate Task<T> AsyncEventMessageFactory<TSaga, T>(BehaviorContext<TSaga> context)
        where TSaga : class, ISaga
        where T : class;
}
