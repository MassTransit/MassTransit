namespace MassTransit.Transports
{
    using System.Collections.Generic;


    /// <summary>
    /// Used to read a header from a transport message
    /// </summary>
    public interface IHeaderProvider
    {
        IEnumerable<KeyValuePair<string, object>> GetAll();

        bool TryGetHeader(string key, out object value);
    }
}
