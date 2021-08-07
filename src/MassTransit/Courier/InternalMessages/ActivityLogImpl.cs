namespace MassTransit.Courier.InternalMessages
{
    using System;
    using Contracts;


    [Serializable]
    class ActivityLogImpl :
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

        public Guid ExecutionId { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public HostInfo Host { get; set; }
    }
}
