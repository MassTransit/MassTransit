namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    [Serializable]
    class RoutingSlipTerminatedMessage :
        RoutingSlipTerminated
    {
        public RoutingSlipTerminatedMessage()
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
