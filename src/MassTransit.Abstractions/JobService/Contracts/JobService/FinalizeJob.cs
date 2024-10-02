namespace MassTransit.Contracts.JobService;

using System;


public interface FinalizeJob
{
    /// <summary>
    /// The job identifier
    /// </summary>
    Guid JobId { get; }
}
