namespace Automatonymous
{
    /// <summary>
    /// Return the name of the event hub
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate string EventHubNameProvider<in TInstance>(ConsumeEventContext<TInstance> context);


    /// <summary>
    /// Return the name of the event hub
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate string EventHubNameProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;
}
