namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    class RoutingSlipTerminatedMessage :
        RoutingSlipTerminated
    {
        protected RoutingSlipTerminatedMessage()
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

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }

        public string ActivityName { get; private set; }
        public Guid ExecutionId { get; private set; }
        public HostInfo Host { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }
        public Activity[] DiscardedItinerary { get; private set; }
    }
}
