namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Defines a message consumer which runs a job asynchronously, without waiting, which is monitored by Conductor
    /// services, to monitor the job, limit concurrency, etc.
    /// </summary>
    /// <typeparam name="TJob">The job message type</typeparam>
    public interface IJobConsumer<in TJob> :
        IConsumer
        where TJob : class
    {
        Task Run(JobContext<TJob> context);
    }
}
