#nullable enable
namespace MassTransit;

using System;
using System.Collections.Generic;


public interface JobTypeInfo
{
    /// <summary>
    /// The job type name, supplied by the job consumer
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Set the concurrent job limit. The limit is applied to each instance if the job consumer is scaled out.
    /// </summary>
    int ConcurrentJobLimit { get; }

    /// <summary>
    /// Job properties configured by <see cref="JobOptions{TJob}" />
    /// </summary>
    IReadOnlyDictionary<string, object> Properties { get; }

    /// <summary>
    /// Currently active jobs for this job type across all instances
    /// </summary>
    IReadOnlyList<ActiveJob> ActiveJobs { get; }

    /// <summary>
    /// Currently active instances for this job type, that aren't suspect/dead
    /// </summary>
    IReadOnlyDictionary<Uri, JobTypeInstance> Instances { get; }
}
