namespace MassTransit
{
    using System;


    public interface IMessageCorrelationId<in T>
        where T : class
    {
        /// <summary>
        /// Get the CorrelationId from the message, if available
        /// </summary>
        /// <param name="message"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        bool TryGetCorrelationId(T message, out Guid correlationId);
    }
}
