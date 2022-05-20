namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    [Serializable]
    public class RoutingSlipTerminatedMessage :
        RoutingSlipTerminated
    {
    #pragma warning disable CS8618
        public RoutingSlipTerminatedMessage()
    #pragma warning restore CS8618
        {
        }

        public RoutingSlipTerminatedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables, IEnumerable<Activity> discardedItinerary)
        {
            Host = host;
            Duration = duration;
            Timestamp = timestamp;

            TrackingNumber = trackingNumber;
            ActivityName = activityName;
            Variables = variables;
            DiscardedItinerary = discardedItinerary.ToArray();
            ExecutionId = executionId;
        }

        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }

        public string ActivityName { get; set; }
        public Guid ExecutionId { get; set; }
        public HostInfo Host { get; set; }

        public IDictionary<string, object> Variables { get; set; }
        public Activity[] DiscardedItinerary { get; set; }
    }
}
