namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    public class RoutingSlipActivityFaultedMessage :
        RoutingSlipActivityFaulted
    {
    #pragma warning disable CS8618
        public RoutingSlipActivityFaultedMessage()
    #pragma warning restore CS8618
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

        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public string ActivityName { get; set; }
        public HostInfo Host { get; set; }
        public Guid ExecutionId { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
        public IDictionary<string, object> Variables { get; set; }
    }
}
