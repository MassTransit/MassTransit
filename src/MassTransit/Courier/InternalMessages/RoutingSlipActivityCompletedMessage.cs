namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class RoutingSlipActivityCompletedMessage :
        RoutingSlipActivityCompleted
    {
        public RoutingSlipActivityCompletedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> arguments,
            IDictionary<string, object> data)
        {
            Host = host;
            Timestamp = timestamp;
            Duration = duration;

            TrackingNumber = trackingNumber;
            ExecutionId = executionId;
            ActivityName = activityName;
            Data = data;
            Variables = variables;
            Arguments = arguments;
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public Guid ExecutionId { get; private set; }
        public string ActivityName { get; private set; }
        public HostInfo Host { get; private set; }
        public IDictionary<string, object> Arguments { get; private set; }
        public IDictionary<string, object> Data { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}
