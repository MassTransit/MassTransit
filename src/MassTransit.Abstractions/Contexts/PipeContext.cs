namespace MassTransit
{
    using System;
    using System.Threading;


    /// <summary>
    /// The base context for all pipe types, includes the payload side-banding of data
    /// with the payload, as well as the cancellationToken to avoid passing it everywhere
    /// </summary>
    public interface PipeContext
    {
        /// <summary>
        /// Used to cancel the execution of the context
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Checks if a payload is present in the context
        /// </summary>
        /// <param name="payloadType"></param>
        /// <returns></returns>
        bool HasPayloadType(Type payloadType);

        /// <summary>
        /// Retrieves a payload from the pipe context
        /// </summary>
        /// <typeparam name="T">The payload type</typeparam>
        /// <param name="payload">The payload</param>
        /// <returns></returns>
        bool TryGetPayload<T>(out T? payload)
            where T : class;

        /// <summary>
        /// Returns an existing payload or creates the payload using the factory method provided
        /// </summary>
        /// <typeparam name="T">The payload type</typeparam>
        /// <param name="payloadFactory">The payload factory is the payload is not present</param>
        /// <returns>The payload</returns>
        T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
            where T : class;

        /// <summary>
        /// Either adds a new payload, or updates an existing payload
        /// </summary>
        /// <param name="addFactory">The payload factory called if the payload is not present</param>
        /// <param name="updateFactory">The payload factory called if the payload already exists</param>
        /// <typeparam name="T">The payload type</typeparam>
        /// <returns></returns>
        T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
            where T : class;
    }
}
