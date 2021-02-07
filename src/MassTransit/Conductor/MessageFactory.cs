namespace MassTransit.Conductor
{
    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public delegate TResult MessageFactory<in TInput, out TResult>(OrchestrationContext<TInput> context)
        where TInput : class
        where TResult : class;
}
