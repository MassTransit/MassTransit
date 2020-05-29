namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    class RoutingSlipFaultedMessage :
        RoutingSlipFaulted
    {
        public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration,
            IEnumerable<ActivityException> activityExceptions, IDictionary<string, object> variables)
        {
            TrackingNumber = trackingNumber;
            Timestamp = timestamp;
            Duration = duration;

            Variables = variables;
            ActivityExceptions = activityExceptions.ToArray();
        }

        public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, ActivityException activityException)
        {
            Timestamp = timestamp;
            Duration = duration;

            TrackingNumber = trackingNumber;
            ActivityExceptions = new[] {activityException};
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public ActivityException[] ActivityExceptions { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}
