namespace MassTransit.Turnout.Components
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Contracts.Turnout;


    public interface IJobService
    {
        Uri InstanceAddress { get; }

        /// <summary>
        /// Starts a job
        /// </summary>
        /// <typeparam name="T">The message type that is used to initiate the job</typeparam>
        /// <param name="context">The context of the message being consumed</param>
        /// <param name="job">The job command</param>
        /// <param name="jobFactory">The factory to create the job Task</param>
        /// <param name="timeout">The job timeout, after which the job is cancelled</param>
        /// <returns>The newly created job's handle</returns>
        Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IJobFactory<T> jobFactory, TimeSpan timeout)
            where T : class;

        /// <summary>
        /// Shut town the job service, cancelling any pending jobs
        /// </summary>
        Task Stop();
    }
}
