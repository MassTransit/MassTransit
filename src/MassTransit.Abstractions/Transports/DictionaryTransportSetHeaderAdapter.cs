namespace MassTransit.Transports
{
    using System.Collections.Generic;


    public class DictionaryTransportSetHeaderAdapter :
        ITransportSetHeaderAdapter<object>
    {
        readonly IHeaderValueConverter _converter;
        readonly TransportHeaderOptions _options;

        public DictionaryTransportSetHeaderAdapter(IHeaderValueConverter converter, TransportHeaderOptions options = TransportHeaderOptions.Default)
        {
            _converter = converter;
            _options = options;
        }

        public void Set(IDictionary<string, object> dictionary, in HeaderValue headerValue)
        {
            switch (headerValue.Value)
            {
                case null:
                    if (dictionary.ContainsKey(headerValue.Key))
                        dictionary.Remove(headerValue.Key);
                    break;

                default:
                    if (IsHeaderIncluded(headerValue.Key) && _converter.TryConvert(headerValue, out var result))
                        dictionary[result.Key] = result.Value;
                    break;
            }
        }

        public void Set<T>(IDictionary<string, object> dictionary, in HeaderValue<T> headerValue)
        {
            switch (headerValue.Value)
            {
                case null:
                case string s when string.IsNullOrWhiteSpace(s):
                    if (dictionary.ContainsKey(headerValue.Key))
                        dictionary.Remove(headerValue.Key);
                    break;

                default:
                    if (IsHeaderIncluded(headerValue.Key) && _converter.TryConvert(headerValue, out var result))
                        dictionary[result.Key] = result.Value;
                    break;
            }
        }

        bool IsHeaderIncluded(string key)
        {
            if (key.StartsWith("MT-Host-"))
                return _options.HasFlag(TransportHeaderOptions.IncludeHost);

            if (key.StartsWith("MT-Fault-"))
            {
                if (_options.HasFlag(TransportHeaderOptions.IncludeFaultDetail))
                    return true;

                return _options.HasFlag(TransportHeaderOptions.IncludeFaultMessage) && key.Equals(MessageHeaders.FaultMessage);
            }

            return true;
        }
    }
}
