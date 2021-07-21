namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class RoutingSlipActivityFaultedMessage :
        RoutingSlipActivityFaulted
    {
        protected RoutingSlipActivityFaultedMessage()
        {
        }

        public RoutingSlipActivityFaultedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, ExceptionInfo exceptionInfo, IDictionary<string, object> variables,
            IDictionary<string, object> arguments)
        {
            Host = host;
            TrackingNumber = trackingNumber;
            Timestamp = timestamp;
            Duration = duration;
            ExecutionId = executionId;
            ActivityName = activityName;
            Variables = variables;
            Arguments = arguments;
            ExceptionInfo = exceptionInfo;
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string ActivityName { get; private set; }
        public HostInfo Host { get; private set; }
        public Guid ExecutionId { get; private set; }
        public ExceptionInfo ExceptionInfo { get; private set; }
        public IDictionary<string, object> Arguments { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}
