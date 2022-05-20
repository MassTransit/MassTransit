namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    public class RoutingSlipActivityCompensatedMessage :
        RoutingSlipActivityCompensated
    {
    #pragma warning disable CS8618
        public RoutingSlipActivityCompensatedMessage()
    #pragma warning restore CS8618
        {
        }

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

        public Guid ExecutionId { get; set; }
        public HostInfo Host { get; set; }
        public IDictionary<string, object> Data { get; set; }
        public IDictionary<string, object> Variables { get; set; }
        public TimeSpan Duration { get; set; }
        public string ActivityName { get; set; }
        public Guid TrackingNumber { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
