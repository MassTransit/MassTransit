namespace MassTransit.Turnout.Commands
{
    using System;
    using Contracts;


    class Supervise :
        SuperviseJob
    {
        public Supervise(Guid jobId, DateTime lastStatusTimestamp, JobStatus lastStatus)
        {
            JobId = jobId;
            LastStatusTimestamp = lastStatusTimestamp;
            LastStatus = lastStatus;
        }

        public Guid JobId { get; private set; }

        public DateTime LastStatusTimestamp { get; private set; }

        public JobStatus LastStatus { get; private set; }
    }
}