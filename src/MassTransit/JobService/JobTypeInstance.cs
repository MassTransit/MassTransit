#nullable enable
namespace MassTransit;

using System;
using System.Collections.Generic;


public class JobTypeInstance
{
    Dictionary<string, object>? _properties;

    public DateTime? Updated { get; set; }
    public DateTime? Used { get; set; }

    public Dictionary<string, object> Properties
    {
        get => _properties ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        set => _properties = value;
    }

    /// <summary>
    /// Sets properties on the job consumer instance that can be used by a custom job distribution strategy
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
    /// Sets a property to the job consumer instance that can be used by a custom job distribution strategy
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
    /// Sets a property to the job consumer instance that can be used by a custom job distribution strategy
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
}
