namespace MassTransit.JobService.Configuration
{
    using System;
    using Components.StateMachines;
    using Saga;


    public interface IJobServiceConfigurator
    {
        /// <summary>
        /// Sets the job saga repository (default is in-memory, which is not recommended for production).
        /// The job repository is used to keep track of all job types, and tracking running jobs.
        /// </summary>
        ISagaRepository<JobTypeSaga> Repository { set; }

        /// <summary>
        /// Sets the job state saga repository (default is in-memory, which is not recommended for production).
        /// Used to keep track of every job that was run.
        /// </summary>
        ISagaRepository<JobSaga> JobRepository { set; }

        /// <summary>
        /// Sets the job attempt state saga repository (default is in-memory, which is not recommended for production).
        /// Used to keep track of each job attempt, which may be retried based upon a retry policy.
        /// </summary>
        ISagaRepository<JobAttemptSaga> JobAttemptRepository { set; }

        /// <summary>
        /// Override the default turnout state endpoint name (defaults to TurnoutState, turnout_state, or turnout-state)
        /// </summary>
        string JobServiceStateEndpointName { set; }

        /// <summary>
        /// Override the default turnout state endpoint name (defaults to TurnoutJobState, turnout_job_state, or turnout-job-state)
        /// </summary>
        string JobServiceJobStateEndpointName { set; }

        /// <summary>
        /// Override the default turnout state endpoint name (defaults to TurnoutJobAttemptState, turnout_job_attempt_state, or turnout-job-attempt-state)
        /// </summary>
        string JobServiceJobAttemptStateEndpointName { set; }

        /// <summary>
        /// The time to wait before attempting to allocate a job slot when no slots are available
        /// </summary>
        TimeSpan JobSlotWaitTime { set; }

        /// <summary>
        /// Time to wait before checking the status of a job to ensure it is still running (not dead)
        /// </summary>
        TimeSpan JobStatusCheckInterval { set; }
    }
}
