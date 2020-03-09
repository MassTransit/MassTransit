namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Link a service endpoint at runtime, which allows endpoints to respond with their Up status.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface Link<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Uniquely identifies the client which is attempting to Link
        /// </summary>
        Guid ClientId { get; }
    }
}
