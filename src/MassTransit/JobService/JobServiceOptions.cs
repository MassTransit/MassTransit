namespace MassTransit.JobService
{
    using System;
    using System.Collections.Generic;
    using Components;
    using GreenPipes;
    using MassTransit.Configuration;


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
            StartJobTimeout = TimeSpan.FromMinutes(2);
            SlotRequestTimeout = TimeSpan.FromSeconds(10);
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
        public Uri JobSagaEndpointAddress { get; private set; }

        /// <summary>
        /// The endpoint for the JobAttemptStateMachine
        /// </summary>
        public Uri JobTypeSagaEndpointAddress { get; private set; }

        /// <summary>
        /// The endpoint for the JobAttemptStateMachine
        /// </summary>
        public Uri JobAttemptSagaEndpointAddress { get; private set; }

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

        public void Set(JobServiceOptions options)
        {
            JobService = options.JobService;
            SlotRequestTimeout = options.SlotRequestTimeout;
            SlotWaitTime = options.SlotWaitTime;
            StartJobTimeout = options.StartJobTimeout;
            StatusCheckInterval = options.StatusCheckInterval;

            JobSagaEndpointAddress = options.JobSagaEndpointAddress;
            JobAttemptSagaEndpointAddress = options.JobAttemptSagaEndpointAddress;
            JobTypeSagaEndpointAddress = options.JobTypeSagaEndpointAddress;

            _jobAttemptSagaEndpointName = options._jobAttemptSagaEndpointName;
            _jobSagaEndpointName = options._jobSagaEndpointName;
            _jobTypeSagaEndpointName = options._jobTypeSagaEndpointName;
        }
    }
}
