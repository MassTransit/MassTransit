namespace MassTransit
{
    using System;


    public class OutboxDeliveryServiceOptions
    {
        /// <summary>
        /// The number of message to deliver at a time from an individual outbox
        /// </summary>
        public int MessageDeliveryLimit { get; set; } = 100;

        /// <summary>
        /// Transport Send timeout when delivering messages to the transport
        /// </summary>
        public TimeSpan MessageDeliveryTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Delay between each database sweep to load outbox messages
        /// </summary>
        public TimeSpan QueryDelay { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The number of outboxes to deliver concurrently (set to 1 if you need in-order delivery across multiple transactions)
        /// </summary>
        public int QueryMessageLimit { get; set; } = 100;

        /// <summary>
        /// Database query timeout for loading outbox messages
        /// </summary>
        public TimeSpan QueryTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
