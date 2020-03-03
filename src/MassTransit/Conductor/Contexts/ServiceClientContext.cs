namespace MassTransit.Conductor.Contexts
{
    using System;
    using GreenPipes;


    /// <summary>
    /// Used by a service endpoint to track clients. Capabilities may add payloads
    /// to keep track of their own metrics, limits, etc.
    /// </summary>
    public interface ServiceClientContext :
        PipeContext
    {
        /// <summary>
        /// Unique identifier for the service client
        /// </summary>
        Guid ClientId { get; }

        /// <summary>
        /// The client address
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// Notify the context that a message was consumed on behalf of the client
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        void NotifyConsumed<T>(ConsumeContext<T> context)
            where T : class;
    }
}
