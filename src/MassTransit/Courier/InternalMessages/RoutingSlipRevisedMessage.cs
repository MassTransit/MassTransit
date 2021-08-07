namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    [Serializable]
    class RoutingSlipRevisedMessage :
        RoutingSlipRevised
    {
        public RoutingSlipRevisedMessage()
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

        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }

        public string ActivityName { get; set; }
        public Guid ExecutionId { get; set; }
        public HostInfo Host { get; set; }

        public IDictionary<string, object> Variables { get; set; }

        public Activity[] Itinerary { get; set; }

        public Activity[] DiscardedItinerary { get; set; }
    }
}
