namespace MassTransit.Turnout.Events
{
    using System;
    using Contracts;


    class Completed :
        JobCompleted
    {
        public Completed(Guid jobId)
        {
            JobId = jobId;
            Timestamp = DateTime.UtcNow;
        }

        public Guid JobId { get; }
        public DateTime Timestamp { get; }
    }
}