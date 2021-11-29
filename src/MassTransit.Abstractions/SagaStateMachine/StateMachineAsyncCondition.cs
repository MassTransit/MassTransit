namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Filters activities based on the async conditional statement
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<bool> StateMachineAsyncCondition<TSaga, in TMessage>(BehaviorContext<TSaga, TMessage> context)
        where TSaga : class, ISaga
        where TMessage : class;


    /// <summary>
    /// Filters activities based on the async conditional statement
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task<bool> StateMachineAsyncCondition<TSaga>(BehaviorContext<TSaga> context)
        where TSaga : class, ISaga;
}
