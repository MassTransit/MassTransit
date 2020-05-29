namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class RoutingSlipCompletedMessage :
        RoutingSlipCompleted
    {
        public RoutingSlipCompletedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables)
        {
            Duration = duration;
            Timestamp = timestamp;

            TrackingNumber = trackingNumber;
            Variables = variables;
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}
