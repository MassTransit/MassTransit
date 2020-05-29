namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class RoutingSlipActivityCompensatedMessage :
        RoutingSlipActivityCompensated
    {
        public RoutingSlipActivityCompensatedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> data)
        {
            Host = host;
            Duration = duration;
            Timestamp = timestamp;

            TrackingNumber = trackingNumber;
            ExecutionId = executionId;
            ActivityName = activityName;
            Data = data;
            Variables = variables;
        }

        public Guid ExecutionId { get; private set; }
        public HostInfo Host { get; private set; }
        public IDictionary<string, object> Data { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string ActivityName { get; private set; }
        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
    }
}
