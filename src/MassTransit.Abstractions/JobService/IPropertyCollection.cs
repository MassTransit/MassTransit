namespace MassTransit;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


public interface IPropertyCollection :
    IReadOnlyDictionary<string, object>
{
    /// <summary>
    /// If the specified property name is found, returns the value of the property as an object
    /// </summary>
    /// <param name="key">The property name</param>
    /// <param name="value">The output property value</param>
    /// <returns>True if the property is present, otherwise false</returns>
    bool TryGet(string key, [NotNullWhen(true)] out object? value);

    /// <summary>
    /// Returns the specified property as the type, returning a default value is the property is not found
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    /// <param name="key">The property name</param>
    /// <param name="defaultValue">The default value of the property if not found</param>
    /// <returns>The property value</returns>
    T? Get<T>(string key, T? defaultValue = default)
        where T : class;

    /// <summary>
    /// Returns the specified property as the type, returning a default value is the property is not found
    /// </summary>
    /// <typeparam name="T">The result type</typeparam>
    /// <param name="key">The property name</param>
    /// <param name="defaultValue">The default value of the property if not found</param>
    /// <returns>The property value</returns>
    T? Get<T>(string key, T? defaultValue = default)
        where T : struct;
}
