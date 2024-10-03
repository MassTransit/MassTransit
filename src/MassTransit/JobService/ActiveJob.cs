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
    Dictionary<string, object>? _properties;
    public Guid JobId { get; set; }

    /// <summary>
    /// Calculated from the JobTimeout based on the time the job slot was requested, not currently used
    /// </summary>
    public DateTime Deadline { get; set; }

    /// <summary>
    /// The instance assigned to the job
    /// </summary>
    public Uri InstanceAddress { get; set; } = null!;

    /// <summary>
    /// Properties associated with the job, for use by job distribution strategies
    /// </summary>
    public Dictionary<string, object> Properties
    {
        get => _properties ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        set => _properties = value;
    }

    public bool Equals(ActiveJob? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return JobId.Equals(other.JobId);
    }

    /// <summary>
    /// Sets properties on the job that can be used by a custom job distribution strategy
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="overwrite"></param>
    /// <returns></returns>
    public void SetProperties(IEnumerable<KeyValuePair<string, object?>>? properties, bool overwrite = true)
    {
        if (properties != null)
        {
            foreach (KeyValuePair<string, object?> header in properties)
                SetProperty(header.Key, header.Value, overwrite);
        }
    }

    /// <summary>
    /// Sets a property to the job that can be used by a custom job distribution strategy
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetProperty(string key, string? value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            Properties.Remove(key);
        else
            Properties[key] = value;
    }

    /// <summary>
    /// Sets a property to the job that can be used by a custom job distribution strategy
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="overwrite"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetProperty(string key, object? value, bool overwrite = true)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (overwrite)
        {
            if (value == null)
                Properties.Remove(key);
            else
                Properties[key] = value;
        }
        else if (value != null && !Properties.ContainsKey(key))
            Properties.Add(key, value);
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
