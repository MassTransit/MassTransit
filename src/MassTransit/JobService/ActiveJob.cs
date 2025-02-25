#nullable enable
namespace MassTransit;

using System;
using System.Collections.Generic;


/// <summary>
/// Active Jobs are allocated a concurrency slot, and are valid until the deadline is reached, after
/// which they may be automatically released.
/// </summary>
public class ActiveJob :
    IEquatable<ActiveJob>
{
    public Guid JobId { get; set; }

    /// <summary>
    /// Calculated from the JobTimeout based on the time the job slot was requested, not currently used
    /// </summary>
    public DateTime Deadline { get; set; }

    /// <summary>
    /// The instance assigned to the job
    /// </summary>
    public Uri InstanceAddress { get; set; } = null!;

    public Dictionary<string, object>? Properties { get; set; }

    public bool Equals(ActiveJob? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return JobId.Equals(other.JobId);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((ActiveJob)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return JobId.GetHashCode();
    }
}
