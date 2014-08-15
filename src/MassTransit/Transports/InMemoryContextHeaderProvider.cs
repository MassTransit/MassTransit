namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using Context;


    public class InMemoryContextHeaderProvider :
        IContextHeaderProvider
    {
        readonly IDictionary<string, object> _headers;

        public InMemoryContextHeaderProvider(IDictionary<string, object> headers)
        {
            _headers = headers;
        }

        public bool TryGetHeader(string key, out object value)
        {
            return _headers.TryGetValue(key, out value);
        }
    }
}