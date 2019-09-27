namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    public class ActivityCompensationFailed :
        RoutingSlipActivityCompensationFailed
    {
        readonly TimeSpan _duration;
        readonly DateTime _timestamp;

        public ActivityCompensationFailed(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data)
        {
            Host = host;
            _duration = duration;
            _timestamp = timestamp;

            TrackingNumber = trackingNumber;
            ExecutionId = executionId;
            ActivityName = activityName;
            Data = data;
            Variables = variables;
            ExceptionInfo = exceptionInfo;
        }

        public Guid TrackingNumber { get; }

        DateTime RoutingSlipActivityCompensationFailed.Timestamp => _timestamp;

        public Guid ExecutionId { get; }
        public string ActivityName { get; }
        public IDictionary<string, object> Data { get; }
        public ExceptionInfo ExceptionInfo { get; }
        public IDictionary<string, object> Variables { get; }

        TimeSpan RoutingSlipActivityCompensationFailed.Duration => _duration;

        public HostInfo Host { get; }
    }
}