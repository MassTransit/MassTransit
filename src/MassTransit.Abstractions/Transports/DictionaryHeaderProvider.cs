namespace MassTransit.Transports
{
    using System.Collections.Generic;


    /// <summary>
    /// A simple in-memory header collection for use with the in memory transport
    /// </summary>
    public class DictionaryHeaderProvider :
        IHeaderProvider
    {
        readonly IDictionary<string, object> _headers;

        public DictionaryHeaderProvider(IDictionary<string, object>? headers = default)
        {
            _headers = headers ?? new Dictionary<string, object>();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers;
        }

        public bool TryGetHeader(string key, out object value)
        {
            return _headers.TryGetValue(key, out value);
        }
    }
}
