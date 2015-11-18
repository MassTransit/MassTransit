namespace MassTransit.Turnout.Events
{
    using System;
    using Contracts;
    using MassTransit.Events;


    class Faulted :
        JobFaulted
    {
        public Faulted(Guid jobId, Exception exception)
        {
            JobId = jobId;
            Timestamp = DateTime.UtcNow;
            Exceptions = new FaultExceptionInfo(exception);
        }

        public Guid JobId { get; }
        public DateTime Timestamp { get; }
        public ExceptionInfo Exceptions { get; }
    }
}