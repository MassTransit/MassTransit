namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;


    public class OnRampDeliveryOptions
    {
        /// <summary>
        /// The number of messages to query from the outbox on each query
        /// </summary>
        public int MessageLimit { get; set; } = 32;

        /// <summary>
        /// The number of message to deliver at a time from the outbox
        /// </summary>
        public int DeliveryLimit { get; set; } = 32;

        /// <summary>
        /// Database query timeout for loading outbox messages
        /// </summary>
        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Transport Send timeout when delivering messages to the transport
        /// </summary>
        public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Delay between each database sweep to load outbox messages
        /// </summary>
        public TimeSpan SweepInterval { get; set; } = TimeSpan.FromSeconds(5);
    }
}
