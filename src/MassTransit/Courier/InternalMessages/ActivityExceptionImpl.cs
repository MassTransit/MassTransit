namespace MassTransit.Courier.InternalMessages
{
    using System;
    using Contracts;


    class ActivityExceptionImpl :
        ActivityException
    {
        public ActivityExceptionImpl(string activityName, HostInfo host, Guid executionId, DateTime timestamp, TimeSpan elapsed,
            ExceptionInfo exceptionInfo)
        {
            ExecutionId = executionId;

            Timestamp = timestamp;
            Elapsed = elapsed;
            Name = activityName;
            Host = host;
            ExceptionInfo = exceptionInfo;
        }

        public Guid ExecutionId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Elapsed { get; private set; }
        public string Name { get; private set; }
        public HostInfo Host { get; private set; }
        public ExceptionInfo ExceptionInfo { get; private set; }
    }
}
