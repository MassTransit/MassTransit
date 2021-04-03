namespace MassTransit.Conductor.Contexts
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;


    /// <summary>
    /// Used by a service endpoint to track clients. Capabilities may add payloads
    /// to keep track of their own metrics, limits, etc.
    /// </summary>
    public interface ServiceInstanceContext :
        PipeContext
    {
        /// <summary>
        /// Unique identifier for the service instance
        /// </summary>
        Guid InstanceId { get; }

        /// <summary>
        /// User defined attributes of the service instance
        /// </summary>
        IReadOnlyDictionary<string, string> InstanceAttributes { get; }

        /// <summary>
        /// Endpoint uri of the service instance
        /// </summary>
        Uri Endpoint { get; }

        /// <summary>
        /// The instance start timestamp
        /// </summary>
        DateTime? Started { get; }

        /// <summary>
        /// Notify the context that a message was consumed on behalf of the client
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        void NotifySent<T>(SendContext<T> context)
            where T : class;
    }
}
