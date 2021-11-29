namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;


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
        /// <param name="publishEndpoint"></param>
        Task Stop(IPublishEndpoint publishEndpoint);

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
        /// <param name="jobTypeId"></param>
        /// <typeparam name="T"></typeparam>
        void RegisterJobType<T>(IReceiveEndpointConfigurator configurator, JobOptions<T> options, Guid jobTypeId)
            where T : class;

        Task BusStarted(IPublishEndpoint publishEndpoint);

        /// <summary>
        /// Return the registered JobTypeId for the job type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Guid GetJobTypeId<T>()
            where T : class;
    }
}
