namespace MassTransit.Conductor
{
    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TInput">The data type included in the context</typeparam>
    public delegate object ResultValueProvider<in TInput>(OrchestrationContext<TInput> context)
        where TInput : class;
}
