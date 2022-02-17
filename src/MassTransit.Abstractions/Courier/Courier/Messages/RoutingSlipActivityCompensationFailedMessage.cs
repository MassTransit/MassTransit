namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    public class RoutingSlipActivityCompensationFailedMessage :
        RoutingSlipActivityCompensationFailed
    {
    #pragma warning disable CS8618
        public RoutingSlipActivityCompensationFailedMessage()
    #pragma warning restore CS8618
        {
        }

        public RoutingSlipActivityCompensationFailedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp,
            TimeSpan duration, ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data)
        {
            Host = host;
            Duration = duration;
            Timestamp = timestamp;

            TrackingNumber = trackingNumber;
            ExecutionId = executionId;
            ActivityName = activityName;
            Data = data;
            Variables = variables;
            ExceptionInfo = exceptionInfo;
        }

        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid ExecutionId { get; set; }
        public string ActivityName { get; set; }
        public IDictionary<string, object> Data { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }
        public IDictionary<string, object> Variables { get; set; }
        public TimeSpan Duration { get; set; }
        public HostInfo Host { get; set; }
    }
}
