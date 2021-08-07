namespace MassTransit.Courier.InternalMessages
{
    using System;
    using Contracts;


    [Serializable]
    class ActivityExceptionImpl :
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

        public Guid ExecutionId { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Elapsed { get; set; }
        public string Name { get; set; }
        public HostInfo Host { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }
    }
}
