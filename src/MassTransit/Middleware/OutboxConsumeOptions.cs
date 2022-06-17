namespace MassTransit.Middleware
{
    using System;


    public class OutboxConsumeOptions
    {
        /// <summary>
        /// The generated identifier for the consumer based upon endpoint name
        /// </summary>
        public Guid ConsumerId { get; set; }

        /// <summary>
        /// The number of message to deliver at a time from the outbox
        /// </summary>
        public int MessageDeliveryLimit { get; set; } = 1;
    }
}
