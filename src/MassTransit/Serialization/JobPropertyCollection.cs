#nullable enable
namespace MassTransit.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Internals;


public class JobPropertyCollection :
    ISetPropertyCollection
{
    Dictionary<string, object>? _properties;

    public Dictionary<string, object> Properties
    {
        get => _properties ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        set => _properties = value;
    }

    public int Count => _properties?.Count ?? 0;

    public bool TryGet(string key, [NotNullWhen(true)] out object? value)
    {
        if (_properties != null)
            return _properties.TryGetValue(key, out value);

        value = null;
        return false;
    }

    public T? Get<T>(string key, T? defaultValue = default)
        where T : class
    {
        return _properties == null
            ? defaultValue
            : SystemTextJsonMessageSerializer.Instance.GetValue((IReadOnlyDictionary<string, object>)_properties, key, defaultValue);
    }

    public T? Get<T>(string key, T? defaultValue = default)
        where T : struct
    {
        return _properties == null
            ? defaultValue
            : SystemTextJsonMessageSerializer.Instance.GetValue((IReadOnlyDictionary<string, object>)_properties, key, defaultValue);
    }

    public ISetPropertyCollection Set(string key, string? value)
    {
        Properties.SetValue(key, value);

        return this;
    }

    public ISetPropertyCollection Set(string key, object? value, bool overwrite = true)
    {
        Properties.SetValue(key, value, overwrite);

        return this;
    }

    public ISetPropertyCollection SetMany(IEnumerable<KeyValuePair<string, object?>>? properties, bool overwrite = true)
    {
        Properties.SetValues(properties, overwrite);

        return this;
    }

    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
    {
        // ReSharper disable once NotDisposedResourceIsReturned
        return _properties?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<string, object>>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        // ReSharper disable once NotDisposedResourceIsReturned
        return _properties?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<string, object>>().GetEnumerator();
    }

    int IReadOnlyCollection<KeyValuePair<string, object>>.Count => _properties?.Count ?? 0;

    bool IReadOnlyDictionary<string, object>.ContainsKey(string key)
    {
        return _properties?.ContainsKey(key) ?? false;
    }

    bool IReadOnlyDictionary<string, object>.TryGetValue(string key, [MaybeNullWhen(false)] out object value)
    {
        if (_properties != null)
            return _properties.TryGetValue(key, out value);

        value = null!;
        return false;
    }

    object IReadOnlyDictionary<string, object>.this[string key] => _properties?[key] ?? throw new KeyNotFoundException(key);

    IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => _properties?.Keys ?? Enumerable.Empty<string>();

    IEnumerable<object> IReadOnlyDictionary<string, object>.Values => _properties?.Values ?? Enumerable.Empty<object>();

    public static implicit operator Dictionary<string, object>(JobPropertyCollection properties)
    {
        return properties.Properties;
    }
}
