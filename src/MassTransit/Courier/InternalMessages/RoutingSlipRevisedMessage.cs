namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    class RoutingSlipRevisedMessage :
        RoutingSlipRevised
    {
        protected RoutingSlipRevisedMessage()
        {
        }

        public RoutingSlipRevisedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables,
            IEnumerable<Activity> itinerary, IEnumerable<Activity> discardedItinerary)
        {
            Host = host;
            ActivityName = activityName;
            TrackingNumber = trackingNumber;
            Timestamp = timestamp;
            Duration = duration;
            ExecutionId = executionId;
            Variables = variables;
            Itinerary = itinerary.ToArray();
            DiscardedItinerary = discardedItinerary.ToArray();
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }

        public string ActivityName { get; private set; }
        public Guid ExecutionId { get; private set; }
        public HostInfo Host { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }

        public Activity[] Itinerary { get; private set; }

        public Activity[] DiscardedItinerary { get; private set; }
    }
}
