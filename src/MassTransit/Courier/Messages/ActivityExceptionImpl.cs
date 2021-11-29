namespace MassTransit.Courier.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Contracts;


    [Serializable]
    public class ActivityExceptionImpl :
        ActivityException
    {
        public ActivityExceptionImpl()
        {
        }

        public ActivityExceptionImpl(string activityName, HostInfo host, Guid executionId, DateTime timestamp, TimeSpan elapsed, ExceptionInfo exceptionInfo)
        {
            ExecutionId = executionId;

            Timestamp = timestamp;
            Elapsed = elapsed;
            Name = activityName;
            Host = host;
            ExceptionInfo = exceptionInfo;
        }

        public ActivityExceptionImpl(ActivityException activityException)
        {
            if (string.IsNullOrEmpty(activityException.Name))
                throw new SerializationException("An Activity Name is required");
            if (activityException.ExceptionInfo == null)
                throw new SerializationException("An Activity ExceptionInfo is required");

            ExecutionId = activityException.ExecutionId;
            Timestamp = activityException.Timestamp;
            Elapsed = activityException.Elapsed;
            Name = activityException.Name;
            Host = activityException.Host;
            ExceptionInfo = activityException.ExceptionInfo;
        }

        public Guid ExecutionId { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Elapsed { get; set; }
        public string Name { get; set; }
        public HostInfo Host { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }
    }
}
