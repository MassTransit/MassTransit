namespace MassTransit.Serialization
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class ForwardJsonMessageSerializer :
        IMessageSerializer
    {
        readonly Encoding _encoding;
        readonly JObject _jsonObject;
        readonly ReceiveContext _receiveContext;

        public ForwardJsonMessageSerializer(ReceiveContext receiveContext)
        {
            _receiveContext = receiveContext;
            _encoding = GetMessageEncoding(receiveContext);

            _jsonObject = GetJsonObject(receiveContext, _encoding);
        }

        public ContentType ContentType => _receiveContext.ContentType;

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            SetDestinationAddress(context.DestinationAddress?.ToString());
            UpdateHeaders(context.Headers);

            var body = JsonConvert.SerializeObject(_jsonObject, Formatting.Indented);

            var bytes = _encoding.GetBytes(body);

            stream.Write(bytes, 0, bytes.Length);
        }

        void SetDestinationAddress(string value)
        {
            _jsonObject["destinationAddress"] = value;
        }

        void UpdateHeaders(Headers values)
        {
            var headersToken = _jsonObject["headers"] ?? new JObject();
            var headers = headersToken.ToObject<IDictionary<string, object>>(JsonMessageSerializer.Deserializer);

            foreach (KeyValuePair<string, object> header in values.GetAll())
                headers[header.Key] = header.Value;

            _jsonObject["headers"] = JToken.FromObject(headers);
        }

        static JObject GetJsonObject(ReceiveContext receiveContext, Encoding encoding)
        {
            var bodyString = encoding.GetString(receiveContext.GetBody());

            return JObject.Parse(bodyString);
        }

        static Encoding GetMessageEncoding(ReceiveContext receiveContext)
        {
            var contentEncoding = receiveContext.TransportHeaders.Get("Content-Encoding", default(string));

            return string.IsNullOrWhiteSpace(contentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(contentEncoding);
        }
    }
}
