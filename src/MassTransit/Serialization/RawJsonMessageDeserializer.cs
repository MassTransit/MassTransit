namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class RawJsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly RawSerializerOptions _options;

        public RawJsonMessageDeserializer(JsonSerializer deserializer, RawJsonSerializerOptions options = RawJsonSerializerOptions.Default)
        {
            _deserializer = deserializer;
            _options = (RawSerializerOptions)options;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", RawJsonMessageSerializer.RawJsonContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => RawJsonMessageSerializer.RawJsonContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                var messageEncoding = GetMessageEncoding(receiveContext);

                using var body = receiveContext.GetBodyStream();
                using var reader = new StreamReader(body, messageEncoding, false, 1024, true);
                using var jsonReader = new JsonTextReader(reader);

                var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                return new RawConsumeContext(_deserializer, receiveContext, messageToken, _options);
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException("A JSON serialization exception occurred while deserializing the message", ex);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An exception occurred while deserializing the message", ex);
            }
        }

        static Encoding GetMessageEncoding(ReceiveContext receiveContext)
        {
            var contentEncoding = receiveContext.TransportHeaders.Get("Content-Encoding", default(string));

            return string.IsNullOrWhiteSpace(contentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(contentEncoding);
        }
    }
}
