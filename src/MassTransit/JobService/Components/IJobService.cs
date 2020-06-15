namespace MassTransit.JobService.Components
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Contracts.JobService;


    public interface IJobService
    {
        Uri InstanceAddress { get; }

        /// <summary>
        /// Starts a job
        /// </summary>
        /// <typeparam name="T">The message type that is used to initiate the job</typeparam>
        /// <param name="context">The context of the message being consumed</param>
        /// <param name="job">The job command</param>
        /// <param name="jobPipe">The pipe which executes the job</param>
        /// <param name="timeout">The job timeout, after which the job is cancelled</param>
        /// <returns>The newly created job's handle</returns>
        Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IPipe<ConsumeContext<T>> jobPipe, TimeSpan timeout)
            where T : class;

        /// <summary>
        /// Shut town the job service, cancelling any pending jobs
        /// </summary>
        Task Stop();

        bool TryGetJob(Guid jobId, out JobHandle jobReference);

        /// <summary>
        /// Remove the job from the roster
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobHandle"></param>
        bool TryRemoveJob(Guid jobId, out JobHandle jobHandle);
    }
}
