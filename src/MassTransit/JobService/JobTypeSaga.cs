namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Every job type has one entry in this state machine
    /// </summary>
    public class JobTypeSaga :
        SagaStateMachineInstance,
        ISagaVersion
    {
        public JobTypeSaga()
        {
            ConcurrentJobLimit = 1;

            Instances = new Dictionary<Uri, JobTypeInstance>();
            ActiveJobs = new List<ActiveJob>();
        }

        public int CurrentState { get; set; }

        public int ActiveJobCount { get; set; }

        /// <summary>
        /// The concurrent job limit, which is configured by the job options. Initially, it defaults to one when the state machine
        /// is created. Once a service endpoint starts, that endpoint sends a command to set the configure concurrent job limit.
        /// </summary>
        public int ConcurrentJobLimit { get; set; }

        /// <summary>
        /// The job limit may be overridden temporarily, to either reduce or increase the number of concurrent jobs. Once the
        /// override job limit expires, the concurrent job limit returns to the original value.
        /// </summary>
        public int? OverrideJobLimit { get; set; }

        /// <summary>
        /// If an <see cref="OverrideJobLimit" /> is specified, the time when the override job limit expires
        /// </summary>
        public DateTime? OverrideLimitExpiration { get; set; }

        /// <summary>
        /// The last known active jobs
        /// </summary>
        public List<ActiveJob> ActiveJobs { get; set; }

        /// <summary>
        /// Tracks the instances, when they were last updated
        /// </summary>
        public Dictionary<Uri, JobTypeInstance> Instances { get; set; }

        public byte[] RowVersion { get; set; }

        public int Version { get; set; }

        /// <summary>
        /// The MD5 hash of the job type
        /// </summary>
        public Guid CorrelationId { get; set; }
    }
}
