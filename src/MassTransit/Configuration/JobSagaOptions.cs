namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configuration;


    public class JobSagaOptions :
        JobSagaSettingsConfigurator,
        ISpecification
    {
        Uri _jobAttemptSagaEndpointAddress;
        Uri _jobSagaEndpointAddress;
        Uri _jobTypeSagaEndpointAddress;

        public JobSagaOptions()
        {
            StatusCheckInterval = TimeSpan.FromMinutes(1);
            SlotWaitTime = TimeSpan.FromSeconds(30);
            HeartbeatTimeout = TimeSpan.FromMinutes(5);

            SuspectJobRetryCount = 3;
            ConcurrentMessageLimit = 16;
            FinalizeCompleted = true;
        }

        /// <summary>
        /// The number of concurrent messages
        /// </summary>
        public int? ConcurrentMessageLimit { get; set; }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (SlotWaitTime < TimeSpan.FromSeconds(1))
                yield return this.Failure(nameof(SlotWaitTime), "must be >= 1 second");
            if (StatusCheckInterval < TimeSpan.FromSeconds(30))
                yield return this.Failure(nameof(StatusCheckInterval), "must be >= 30 seconds");
        }

        Uri JobSagaSettingsConfigurator.JobSagaEndpointAddress
        {
            set => _jobSagaEndpointAddress = value;
        }

        Uri JobSagaSettingsConfigurator.JobTypeSagaEndpointAddress
        {
            set => _jobTypeSagaEndpointAddress = value;
        }

        Uri JobSagaSettingsConfigurator.JobAttemptSagaEndpointAddress
        {
            set => _jobAttemptSagaEndpointAddress = value;
        }

        Uri JobSagaSettings.JobAttemptSagaEndpointAddress => _jobAttemptSagaEndpointAddress;
        Uri JobSagaSettings.JobTypeSagaEndpointAddress => _jobTypeSagaEndpointAddress;
        Uri JobSagaSettings.JobSagaEndpointAddress => _jobSagaEndpointAddress;

        /// <summary>
        /// The time to wait for a job slot when one is unavailable
        /// </summary>
        public TimeSpan SlotWaitTime { get; set; }

        /// <summary>
        /// The time after which the status of a job should be checked
        /// </summary>
        public TimeSpan StatusCheckInterval { get; set; }

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
        /// If true, completed jobs will be finalized, removing the saga from the repository
        /// </summary>
        public bool FinalizeCompleted { get; set; }
    }
}
