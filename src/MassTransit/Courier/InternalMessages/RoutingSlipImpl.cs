namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    class RoutingSlipImpl :
        RoutingSlip
    {
        protected RoutingSlipImpl()
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

        public Guid TrackingNumber { get; private set; }
        public DateTime CreateTimestamp { get; private set; }
        public IList<Activity> Itinerary { get; private set; }
        public IList<ActivityLog> ActivityLogs { get; private set; }
        public IList<CompensateLog> CompensateLogs { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
        public IList<ActivityException> ActivityExceptions { get; private set; }
        public IList<Subscription> Subscriptions { get; private set; }
    }
}
