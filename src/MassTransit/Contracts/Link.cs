namespace MassTransit.Contracts
{
    using System;


    /// <summary>
    /// Link a service endpoint at runtime, which allows endpoints to respond with their Up status.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Link<T>
        where T : class
    {
        /// <summary>
        /// Uniquely identifies the client which is attempting to Link
        /// </summary>
        Guid ClientId { get; }
    }
}