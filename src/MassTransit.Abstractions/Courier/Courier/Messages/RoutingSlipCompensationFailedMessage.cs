namespace MassTransit.Courier.Messages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    [Serializable]
    public class RoutingSlipCompensationFailedMessage :
        RoutingSlipCompensationFailed
    {
    #pragma warning disable CS8618
        public RoutingSlipCompensationFailedMessage()
    #pragma warning restore CS8618
        {
        }

        public RoutingSlipCompensationFailedMessage(HostInfo host, Guid trackingNumber, DateTime failureTimestamp, TimeSpan routingSlipDuration,
            ExceptionInfo exceptionInfo,
            IDictionary<string, object> variables)
        {
            Timestamp = failureTimestamp;
            Duration = routingSlipDuration;
            Host = host;

            TrackingNumber = trackingNumber;
            Variables = variables;
            ExceptionInfo = exceptionInfo;
        }

        public Guid TrackingNumber { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }
        public IDictionary<string, object> Variables { get; set; }
        public HostInfo Host { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
