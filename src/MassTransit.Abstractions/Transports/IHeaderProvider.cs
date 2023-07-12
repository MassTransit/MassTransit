namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;


    /// <summary>
    /// Used to read a header from a transport message
    /// </summary>
    public interface IHeaderProvider
    {
        IEnumerable<KeyValuePair<string, object>> GetAll();

        bool TryGetHeader(string key, [NotNullWhen(true)] out object? value);
    }
}
