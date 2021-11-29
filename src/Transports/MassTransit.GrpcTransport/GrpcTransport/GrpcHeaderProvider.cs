namespace MassTransit.GrpcTransport
{
    using System.Collections.Generic;
    using Transports;


    public class GrpcHeaderProvider :
        IHeaderProvider
    {
        readonly Headers _headers;

        public GrpcHeaderProvider(Headers headers)
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
