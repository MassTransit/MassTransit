namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using JobService;


    public class JobServiceOptions :
        IOptions,
        ISpecification
    {
        string _jobAttemptSagaEndpointName;
        string _jobSagaEndpointName;
        string _jobTypeSagaEndpointName;

        public JobServiceOptions()
        {
            StatusCheckInterval = TimeSpan.FromMinutes(1);
            SlotWaitTime = TimeSpan.FromSeconds(30);
            StartJobTimeout = TimeSpan.Zero;
            SlotRequestTimeout = TimeSpan.Zero;
            HeartbeatInterval = TimeSpan.FromMinutes(1);
            HeartbeatTimeout = TimeSpan.FromMinutes(5);

            SuspectJobRetryCount = 3;
            SagaPartitionCount = 16;
        }

        public string JobTypeSagaEndpointName
        {
            get => _jobTypeSagaEndpointName;
            set
            {
                _jobTypeSagaEndpointName = value;
                JobTypeSagaEndpointAddress = new Uri($"queue:{value}");
            }
        }

        public string JobStateSagaEndpointName
        {
            get => _jobSagaEndpointName;
            set
            {
                _jobSagaEndpointName = value;
                JobSagaEndpointAddress = new Uri($"queue:{value}");
            }
        }

        public string JobAttemptSagaEndpointName
        {
            get => _jobAttemptSagaEndpointName;
            set
            {
                _jobAttemptSagaEndpointName = value;
                JobAttemptSagaEndpointAddress = new Uri($"queue:{value}");
            }
        }

        /// <summary>
        /// The endpoint for the JobAttemptStateMachine
        /// </summary>
        public Uri JobSagaEndpointAddress { get; set; }

        /// <summary>
        /// The endpoint for the JobAttemptStateMachine
        /// </summary>
        public Uri JobTypeSagaEndpointAddress { get; set; }

        /// <summary>
        /// The endpoint for the JobAttemptStateMachine
        /// </summary>
        public Uri JobAttemptSagaEndpointAddress { get; set; }

        /// <summary>
        /// The job service for the endpoint
        /// </summary>
        public IJobService JobService { get; set; }

        /// <summary>
        /// Timeout for the Allocate Job Slot Request
        /// </summary>
        public TimeSpan SlotRequestTimeout { get; set; }

        /// <summary>
        /// The time to wait for a job slot when one is unavailable
        /// </summary>
        public TimeSpan SlotWaitTime { get; set; }

        /// <summary>
        /// The time to wait for a job to start
        /// </summary>
        public TimeSpan StartJobTimeout { get; set; }

        /// <summary>
        /// The time after which the status of a job should be checked
        /// </summary>
        public TimeSpan StatusCheckInterval { get; set; }

        /// <summary>
        /// How often a job instance should send a heartbeat
        /// </summary>
        public TimeSpan HeartbeatInterval { get; set; }

        /// <summary>
        /// The time after which an instance will automatically be purged from the instance list
        /// </summary>
        public TimeSpan HeartbeatTimeout { get; set; }

        /// <summary>
        /// The number of times to retry a suspect job before it is faulted. Defaults to zero.
        /// </summary>
        public int SuspectJobRetryCount { get; set; }

        /// <summary>
        /// The delay before retrying a suspect job
        /// </summary>
        public TimeSpan? SuspectJobRetryDelay { get; set; }

        /// <summary>
        /// If specified, overrides the default saga partition count to reduce conflicts when using optimistic concurrency.
        /// If using a saga repository with pessimistic concurrency, this is not recommended.
        /// </summary>
        public int? SagaPartitionCount { get; set; }

        /// <summary>
        /// If true, completed jobs will be finalized, removing the saga from the repository
        /// </summary>
        public bool FinalizeCompleted { get; set; }

        public IReceiveEndpointConfigurator InstanceEndpointConfigurator { get; set; }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (SlotWaitTime < TimeSpan.FromSeconds(1))
                yield return this.Failure(nameof(SlotWaitTime), "must be >= 1 second");
            if (StatusCheckInterval < TimeSpan.FromSeconds(30))
                yield return this.Failure(nameof(StatusCheckInterval), "must be >= 30 seconds");

            if (string.IsNullOrWhiteSpace(JobTypeSagaEndpointName))
                yield return this.Failure(nameof(JobTypeSagaEndpointName), "must not be null or empty");
            if (string.IsNullOrWhiteSpace(JobStateSagaEndpointName))
                yield return this.Failure(nameof(JobStateSagaEndpointName), "must not be null or empty");
            if (string.IsNullOrWhiteSpace(JobAttemptSagaEndpointName))
                yield return this.Failure(nameof(JobAttemptSagaEndpointName), "must not be null or empty");
        }
    }
}
