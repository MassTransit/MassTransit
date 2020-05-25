namespace MassTransit.Turnout
{
    using System;
    using System.Collections.Generic;
    using ConsumeConfigurators;
    using GreenPipes;


    public class TurnoutOptions :
        IOptions,
        ISpecification
    {
        public TurnoutOptions()
        {
            JobStatusCheckInterval = TimeSpan.FromMinutes(1);
            JobSlotWaitTime = TimeSpan.FromSeconds(30);
        }

        string _jobAttemptSagaEndpointName;
        string _jobStateSagaEndpointName;
        string _jobTypeSagaEndpointName;

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
            get => _jobStateSagaEndpointName;
            set
            {
                _jobStateSagaEndpointName = value;
                JobStateSagaEndpointAddress = new Uri($"queue:{value}");
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
        /// The endpoint for the TurnoutJobAttemptStateMachine
        /// </summary>
        public Uri JobStateSagaEndpointAddress { get; private set; }

        /// <summary>
        /// The endpoint for the TurnoutJobAttemptStateMachine
        /// </summary>
        public Uri JobTypeSagaEndpointAddress { get; private set; }

        /// <summary>
        /// The endpoint for the TurnoutJobAttemptStateMachine
        /// </summary>
        public Uri JobAttemptSagaEndpointAddress { get; private set; }

        /// <summary>
        /// The time after which the status of a job should be checked
        /// </summary>
        public TimeSpan JobStatusCheckInterval { get; set; }

        /// <summary>
        /// The time to wait for a job slot when one is unavailable
        /// </summary>
        public TimeSpan JobSlotWaitTime { get; set; }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (JobSlotWaitTime < TimeSpan.FromSeconds(1))
                yield return this.Failure(nameof(JobSlotWaitTime), "must be >= 1 second");
            if (JobStatusCheckInterval < TimeSpan.FromSeconds(30))
                yield return this.Failure(nameof(JobStatusCheckInterval), "must be >= 30 seconds");

            if (string.IsNullOrWhiteSpace(JobTypeSagaEndpointName))
                yield return this.Failure(nameof(JobTypeSagaEndpointName), "must not be null or empty");
            if (string.IsNullOrWhiteSpace(JobStateSagaEndpointName))
                yield return this.Failure(nameof(JobStateSagaEndpointName), "must not be null or empty");
            if (string.IsNullOrWhiteSpace(JobAttemptSagaEndpointName))
                yield return this.Failure(nameof(JobAttemptSagaEndpointName), "must not be null or empty");
        }
    }
}
