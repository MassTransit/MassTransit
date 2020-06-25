namespace MassTransit.JobService.Components
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A JobHandle contains the JobContext, Task, and provides access to the job control
    /// </summary>
    public interface JobHandle
    {
        Guid JobId { get; }

        /// <summary>
        /// The job's status, derived from the underlying Task status
        /// </summary>
        JobStatus Status { get; }

        Task JobTask { get; }

        /// <summary>
        /// Cancel the job task
        /// </summary>
        /// <returns></returns>
        Task Cancel();
    }
}
