namespace MassTransit.MongoDbIntegration.Tests.Courier
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipCompletedEvent :
        RoutingSlipCompleted
    {
        public RoutingSlipCompletedEvent(Guid trackingNumber, DateTime timestamp, TimeSpan duration)
        {
            Timestamp = timestamp;
            Duration = duration;
            TrackingNumber = trackingNumber;

            Variables = new Dictionary<string, object>
            {
                { "Client", 27 },
                { "Reason", "Because I said so" }
            };
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }

        public TimeSpan Duration { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }
    }
}
