namespace MassTransit
{
    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TMessage">The data type included in the context</typeparam>
    public delegate object InitializerValueProvider<in TMessage>(BehaviorContext<FutureState, TMessage> context)
        where TMessage : class;


    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    public delegate object InitializerValueProvider(BehaviorContext<FutureState> context);
}
