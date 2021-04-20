namespace MassTransit.GrpcTransport.Contexts
{
    using System.Collections.Generic;
    using Context;


    public class GrpcHeaderProvider :
        IHeaderProvider
    {
        readonly Headers _headers;

        public GrpcHeaderProvider(Headers headers = default)
        {
            _headers = headers;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers.GetAll();
        }

        public bool TryGetHeader(string key, out object value)
        {
            return _headers.TryGetHeader(key, out value);
        }
    }
}
