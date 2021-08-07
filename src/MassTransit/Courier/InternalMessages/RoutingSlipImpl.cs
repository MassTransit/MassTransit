namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    [Serializable]
    class RoutingSlipImpl :
        RoutingSlip
    {
        public RoutingSlipImpl()
        {
        }

        public RoutingSlipImpl(Guid trackingNumber, DateTime createTimestamp, IEnumerable<Activity> activities,
            IEnumerable<ActivityLog> activityLogs, IEnumerable<CompensateLog> compensateLogs, IEnumerable<ActivityException> exceptions,
            IDictionary<string, object> variables, IEnumerable<Subscription> subscriptions)
        {
            TrackingNumber = trackingNumber;
            CreateTimestamp = createTimestamp;
            Itinerary = activities.ToList();
            ActivityLogs = activityLogs.ToList();
            CompensateLogs = compensateLogs.ToList();
            Variables = variables ?? new Dictionary<string, object>();
            ActivityExceptions = exceptions.ToList();
            Subscriptions = subscriptions.ToList();
        }

        public Guid TrackingNumber { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public IList<Activity> Itinerary { get; set; }
        public IList<ActivityLog> ActivityLogs { get; set; }
        public IList<CompensateLog> CompensateLogs { get; set; }
        public IDictionary<string, object> Variables { get; set; }
        public IList<ActivityException> ActivityExceptions { get; set; }
        public IList<Subscription> Subscriptions { get; set; }
    }
}
