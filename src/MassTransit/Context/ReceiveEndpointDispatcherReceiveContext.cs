namespace MassTransit.Context
{
    using System.Collections.Generic;
    using System.IO;


    public sealed class ReceiveEndpointDispatcherReceiveContext :
        BaseReceiveContext
    {
        readonly byte[] _body;

        public ReceiveEndpointDispatcherReceiveContext(ReceiveEndpointContext receiveEndpointContext, byte[] body, IReadOnlyDictionary<string, object> headers,
            params object[] payloads)
            : base(IsRedelivered(headers), receiveEndpointContext, payloads)
        {
            _body = body;

            HeaderProvider = new ReadOnlyDictionaryHeaderProvider(headers);
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public override byte[] GetBody()
        {
            return _body;
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(GetBody(), false);
        }

        static bool IsRedelivered(IReadOnlyDictionary<string, object> headers)
        {
            if (headers.TryGetValue("DeliveryCount", out var value) && value is int deliveryCount && deliveryCount > 1)
                return true;

            if (headers.TryGetValue("ApproximateReceiveCount", out value) && int.TryParse(value.ToString(), out deliveryCount) && deliveryCount > 1)
                return true;

            return false;
        }


        class ReadOnlyDictionaryHeaderProvider :
            IHeaderProvider
        {
            readonly IReadOnlyDictionary<string, object> _headers;

            public ReadOnlyDictionaryHeaderProvider(IReadOnlyDictionary<string, object> headers = default)
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
}
