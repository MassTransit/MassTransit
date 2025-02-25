namespace MassTransit.Transports;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


/// <summary>
/// A simple in-memory header collection for use with the in memory transport
/// </summary>
public class DictionarySendHeaderProvider :
    IHeaderProvider
{
    readonly SendHeaders _headers;

    public DictionarySendHeaderProvider(SendHeaders headers)
    {
        _headers = headers;
    }

    public IEnumerable<KeyValuePair<string, object>> GetAll()
    {
        return _headers.GetAll();
    }

    public bool TryGetHeader(string key, [NotNullWhen(true)] out object? value)
    {
        return _headers.TryGetHeader(key, out value);
    }
}
