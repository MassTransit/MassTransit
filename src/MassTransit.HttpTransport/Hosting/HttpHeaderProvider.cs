namespace MassTransit.HttpTransport.Hosting
{
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Microsoft.Owin;


    public class HttpHeaderProvider : IHeaderProvider
    {
        readonly IHeaderDictionary _headers;

        public HttpHeaderProvider(IHeaderDictionary headers)
        {
            _headers = headers;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers.Keys.SelectMany(k => _headers[k].Select(v => new KeyValuePair<string, object>(k, v)));
        }

        public bool TryGetHeader(string key, out object value)
        {
            string[] values;
            var result =  _headers.TryGetValue(key, out values);
            value = values;
            return result;
        }
    }
}