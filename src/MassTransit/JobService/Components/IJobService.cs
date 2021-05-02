namespace MassTransit.JobService.Components
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using GreenPipes;


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
        /// <param name="bus"></param>
        Task Stop(IBus bus);

        bool TryGetJob(Guid jobId, out JobHandle jobReference);

        /// <summary>
        /// Remove the job from the roster
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobHandle"></param>
        bool TryRemoveJob(Guid jobId, out JobHandle jobHandle);

        /// <summary>
        /// Registers a job type at bus configuration time so that the options can be announced when the bus is started/stopped
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterJobType<T>(IReceiveEndpointConfigurator configurator, JobOptions<T> options)
            where T : class;

        Task BusStarted(IBus bus);
    }
}
