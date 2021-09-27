namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A request handle manages the client-side request, and allows the request to be configured, response types added, etc. The handle
    /// should be disposed once it is no longer in-use, and the request has been completed (successfully, or otherwise).
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    public interface RequestHandle<TRequest> :
        RequestHandle,
        IRequestPipeConfigurator<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// The request message that was/will be sent.
        /// </summary>
        Task<TRequest> Message { get; }
    }


    public interface RequestHandle :
        IDisposable
    {
        /// <summary>
        /// The RequestId assigned to the request, and used in the header for the outgoing request message
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// Set the request message time to live, which by default is equal to the request timeout. Clearing this value
        /// will prevent any TimeToLive value from being specified.
        /// </summary>
        RequestTimeout TimeToLive { set; }

        /// <summary>
        /// If the specified result type is present, it is returned.
        /// </summary>
        /// <param name="readyToSend">If true, sets the request as ready to send and sends it</param>
        /// <typeparam name="T">The result type</typeparam>
        /// <returns>True if the result type specified is present, otherwise false</returns>
        Task<Response<T>> GetResponse<T>(bool readyToSend = true)
            where T : class;

        /// <summary>
        /// Cancel the request
        /// </summary>
        void Cancel();
    }
}
