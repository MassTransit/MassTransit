namespace MassTransit.Futures
{
    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="T"></typeparam>
    public delegate T FutureMessageFactory<in TMessage, out T>(FutureConsumeContext<TMessage> context)
        where TMessage : class
        where T : class;


    /// <summary>
    /// Given the event context and request, returns an object used to complete the initialization of the object type
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="T"></typeparam>
    public delegate T FutureMessageFactory<out T>(FutureConsumeContext context)
        where T : class;
}