namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    [Serializable]
    public class RoutingSlipFaultedMessage :
        RoutingSlipFaulted
    {
    #pragma warning disable CS8618
        public RoutingSlipFaultedMessage()
    #pragma warning restore CS8618
        {
        }

        public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration,
            IEnumerable<ActivityException> activityExceptions, IDictionary<string, object> variables)
        {
            TrackingNumber = trackingNumber;
            Timestamp = timestamp;
            Duration = duration;

            ActivityExceptions = activityExceptions.ToArray();
            Variables = variables;
        }

        public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, ActivityException activityException)
        {
            Timestamp = timestamp;
            Duration = duration;

            TrackingNumber = trackingNumber;
            ActivityExceptions = new[] { activityException };
            Variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public ActivityException[] ActivityExceptions { get; set; }
        public IDictionary<string, object> Variables { get; set; }
    }
}
