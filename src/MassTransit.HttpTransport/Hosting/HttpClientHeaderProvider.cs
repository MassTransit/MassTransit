namespace MassTransit.HttpTransport.Hosting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using Context;


    public class HttpClientHeaderProvider : IHeaderProvider
    {
        readonly HttpResponseHeaders _headers;

        public HttpClientHeaderProvider(HttpResponseHeaders headers)
        {
            _headers = headers;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers.SelectMany(k => k.Value.Select(v => new KeyValuePair<string, object>(k.Key, v)));
        }

        public bool TryGetHeader(string key, out object value)
        {
            IEnumerable<string> values;
            var result = _headers.TryGetValues(key, out values);
            value = values?.FirstOrDefault();
            return result;
        }
    }
}