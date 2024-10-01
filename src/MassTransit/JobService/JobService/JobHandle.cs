#nullable enable
namespace MassTransit.JobService
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A JobHandle contains the JobContext, Task, and provides access to the job control
    /// </summary>
    public interface JobHandle :
        IAsyncDisposable
    {
        Guid JobId { get; }

        Task JobTask { get; }

        /// <summary>
        /// Cancel the job task
        /// </summary>
        /// <returns></returns>
        Task Cancel(string? reason);
    }
}
