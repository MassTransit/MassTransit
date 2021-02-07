namespace MassTransit.Conductor
{
    using System.Threading.Tasks;


    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public delegate Task<TResult> AsyncMessageFactory<in TInput, TResult>(OrchestrationContext<TInput> context)
        where TInput : class
        where TResult : class;
}
