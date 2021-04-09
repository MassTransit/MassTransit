using MassTransit.EventStoreDbIntegration;

namespace Automatonymous
{
    /// <summary>
    /// Return the name of the EventStoreDB stream
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate StreamName EventStoreDbStreamNameProvider<in TInstance>(ConsumeEventContext<TInstance> context);


    /// <summary>
    /// Return the name of the EventStoreDB stream
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate StreamName EventStoreDbStreamNameProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;
}
