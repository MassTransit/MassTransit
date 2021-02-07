namespace MassTransit.Futures
{
    using System.Threading.Tasks;


    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public delegate Task<TMessage> AsyncFutureMessageFactory<in TInput, TMessage>(FutureConsumeContext<TInput> context)
        where TInput : class
        where TMessage : class;


    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TMessage"></typeparam>
    public delegate Task<TMessage> AsyncFutureMessageFactory<TMessage>(FutureConsumeContext context)
        where TMessage : class;
}
