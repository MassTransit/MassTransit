namespace MassTransit.Contracts
{
    using System;


    public interface Unlink<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Uniquely identifies the client which is shutting down
        /// </summary>
        Guid ClientId { get; }
    }
}
