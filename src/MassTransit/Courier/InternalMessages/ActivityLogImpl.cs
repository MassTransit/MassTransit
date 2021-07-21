namespace MassTransit.Courier.InternalMessages
{
    using System;
    using Contracts;


    class ActivityLogImpl :
        ActivityLog
    {
        protected ActivityLogImpl()
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

        public Guid ExecutionId { get; private set; }
        public string Name { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public HostInfo Host { get; private set; }
    }
}
