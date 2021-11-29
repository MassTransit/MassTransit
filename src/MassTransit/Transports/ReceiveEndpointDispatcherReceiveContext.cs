namespace MassTransit.Transports
{
    using System.Collections.Generic;


    public sealed class ReceiveEndpointDispatcherReceiveContext :
        BaseReceiveContext
    {
        public ReceiveEndpointDispatcherReceiveContext(ReceiveEndpointContext receiveEndpointContext, byte[] body, IReadOnlyDictionary<string, object> headers,
            params object[] payloads)
            : base(IsRedelivered(headers), receiveEndpointContext, payloads)
        {
            Body = new BytesMessageBody(body);

            HeaderProvider = new ReadOnlyDictionaryHeaderProvider(headers);
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public override MessageBody Body { get; }

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
