namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// A sent request, that may be in-process until the request task is completed
    /// </summary>
    /// <typeparam name="TRequest">The request message type</typeparam>
    public interface Request<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// An awaitable Task that is completed when the request is completed, or faulted
        /// in the case of an error or timeout
        /// </summary>
        Task<TRequest> Task { get; }
    }
}
