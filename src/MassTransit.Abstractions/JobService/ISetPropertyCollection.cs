namespace MassTransit;

using System;
using System.Collections.Generic;


public interface ISetPropertyCollection :
    IPropertyCollection
{
    /// <summary>
    /// Sets a property
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value">The new value, or null to remove the property</param>
    /// <exception cref="ArgumentNullException"></exception>
    ISetPropertyCollection Set(string key, string? value);

    /// <summary>
    /// Sets a property, overwriting an existing value if <paramref name="overwrite" /> is true
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value">The new value, or null to remove the property</param>
    /// <param name="overwrite"></param>
    /// <exception cref="ArgumentNullException"></exception>
    ISetPropertyCollection Set(string key, object? value, bool overwrite = true);

    /// <summary>
    /// Set multiple properties from an existing collection, any null values a removed from the property collection
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="overwrite"></param>
    /// <returns></returns>
    ISetPropertyCollection SetMany(IEnumerable<KeyValuePair<string, object?>>? properties, bool overwrite = true);
}
