namespace MassTransit.SqlTransport
{
    using System;


    public interface EntitySettings
    {
        string EntityName { get; }

        /// <summary>
        /// Idle time before queue should be deleted (consumer-idle, not producer)
        /// </summary>
        TimeSpan? AutoDeleteOnIdle { get; }
    }
}
