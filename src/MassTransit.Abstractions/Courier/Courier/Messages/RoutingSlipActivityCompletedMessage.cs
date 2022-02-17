namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    public class RoutingSlipActivityCompletedMessage :
        RoutingSlipActivityCompleted
    {
    #pragma warning disable CS8618
        public RoutingSlipActivityCompletedMessage()
    #pragma warning restore CS8618
        {
        }

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

        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid ExecutionId { get; set; }
        public string ActivityName { get; set; }
        public HostInfo Host { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
        public IDictionary<string, object> Data { get; set; }
        public IDictionary<string, object> Variables { get; set; }
    }
}
