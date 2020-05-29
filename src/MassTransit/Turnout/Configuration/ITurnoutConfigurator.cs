namespace MassTransit.Turnout.Configuration
{
    using System;
    using Components.StateMachines;
    using MassTransit.Contracts.Turnout;
    using Saga;


    public interface ITurnoutConfigurator
    {
        /// <summary>
        /// Sets the job saga repository (default is in-memory, which is not recommended for production).
        /// The job repository is used to keep track of all job types, and tracking running jobs.
        /// </summary>
        ISagaRepository<TurnoutJobTypeState> Repository { set; }

        /// <summary>
        /// Sets the job state saga repository (default is in-memory, which is not recommended for production).
        /// Used to keep track of every job that was run.
        /// </summary>
        ISagaRepository<TurnoutJobState> JobRepository { set; }

        /// <summary>
        /// Sets the job attempt state saga repository (default is in-memory, which is not recommended for production).
        /// Used to keep track of each job attempt, which may be retried based upon a retry policy.
        /// </summary>
        ISagaRepository<TurnoutJobAttemptState> JobAttemptRepository { set; }

        /// <summary>
        /// Override the default turnout state endpoint name (defaults to TurnoutState, turnout_state, or turnout-state)
        /// </summary>
        string TurnoutStateEndpointName { set; }

        /// <summary>
        /// Override the default turnout state endpoint name (defaults to TurnoutJobState, turnout_job_state, or turnout-job-state)
        /// </summary>
        string TurnoutJobStateEndpointName { set; }

        /// <summary>
        /// Override the default turnout state endpoint name (defaults to TurnoutJobAttemptState, turnout_job_attempt_state, or turnout-job-attempt-state)
        /// </summary>
        string TurnoutJobAttemptStateEndpointName { set; }

        /// <summary>
        /// The time to wait before attempting to allocate a job slot when no slots are available
        /// </summary>
        TimeSpan JobSlotWaitTime { set; }

        /// <summary>
        /// Time to wait before checking the status of a job to ensure it is still running (not dead)
        /// </summary>
        TimeSpan JobStatusCheckInterval { set; }

        /// <summary>
        /// Configures a turnout job. Jobs are started by using the request or service client to send
        /// <see cref="SubmitJob{TJob}" /> requests to the endpoint. The default endpoint name is the
        /// message type formatted using the configured <see cref="IEndpointNameFormatter" />.
        /// </summary>
        /// <param name="configure">Job configuration method</param>
        /// <typeparam name="T">The job type, which is the message type that starts a new job.</typeparam>
        void Job<T>(Action<ITurnoutJobConfigurator<T>> configure)
            where T : class;
    }
}
