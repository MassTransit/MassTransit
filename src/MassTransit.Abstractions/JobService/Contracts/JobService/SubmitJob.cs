namespace MassTransit.Contracts.JobService
{
    using System;


    public interface SubmitJob<out TJob>
        where TJob : class
    {
        Guid JobId { get; }

        TJob Job { get; }
    }
}
