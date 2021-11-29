namespace MassTransit.Courier.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Contracts;


    [Serializable]
    public class ActivityLogImpl :
        ActivityLog
    {
        public ActivityLogImpl()
        {
        }

        public ActivityLogImpl(HostInfo host, Guid executionId, string name, DateTime timestamp, TimeSpan duration)
        {
            ExecutionId = executionId;
            Name = name;
            Timestamp = timestamp;
            Duration = duration;
            Host = host;
        }

        public ActivityLogImpl(ActivityLog activityLog)
        {
            if (string.IsNullOrEmpty(activityLog.Name))
                throw new SerializationException("An ActivityLog Name is required");

            ExecutionId = activityLog.ExecutionId;
            Name = activityLog.Name;
            Timestamp = activityLog.Timestamp;
            Duration = activityLog.Duration;
            Host = activityLog.Host;
        }


        public Guid ExecutionId { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public HostInfo Host { get; set; }
    }
}
