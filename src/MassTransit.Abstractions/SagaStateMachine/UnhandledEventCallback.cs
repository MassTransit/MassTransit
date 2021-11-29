namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Callback for an unhandled event in the state machine
    /// </summary>
    /// <typeparam name="TSaga">The state machine instance type</typeparam>
    /// <param name="context">The event context</param>
    /// <returns></returns>
    public delegate Task UnhandledEventCallback<TSaga>(UnhandledEventContext<TSaga> context)
        where TSaga : class, ISaga;
}
