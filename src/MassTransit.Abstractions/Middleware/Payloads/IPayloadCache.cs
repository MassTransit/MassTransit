namespace MassTransit.Payloads
{
    using System;


    /// <summary>
    /// The context properties
    /// </summary>
    public interface IPayloadCache
    {
        /// <summary>
        /// Checks if the property exists in the cache
        /// </summary>
        /// <param name="payloadType">The property type</param>
        /// <returns>True if the property exists in the cache, otherwise false</returns>
        bool HasPayloadType(Type payloadType);

        /// <summary>
        /// Returns the value of the property if it exists in the cache
        /// </summary>
        /// <typeparam name="TPayload">The property type</typeparam>
        /// <param name="payload">The property value</param>
        /// <returns>True if the value was returned, otherwise false</returns>
        bool TryGetPayload<TPayload>(out TPayload? payload)
            where TPayload : class;

        /// <summary>
        /// Return an existing or create a new property
        /// </summary>
        /// <param name="payloadFactory"></param>
        /// <returns></returns>
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
