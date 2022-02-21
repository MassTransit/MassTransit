#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NewtonsoftRawJsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;
        readonly RawSerializerOptions _options;

        public NewtonsoftRawJsonMessageDeserializer(JsonSerializer deserializer, RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _deserializer = deserializer;
            _options = options;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", ContentType);
        }

        public ContentType ContentType => NewtonsoftRawJsonMessageSerializer.RawJsonContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                using var stream = body.GetStream();
                using var reader = new StreamReader(stream, MessageDefaults.Encoding, false, 1024, true);
                using var jsonReader = new JsonTextReader(reader);

                var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                var messageContext = new RawMessageContext(headers, destinationAddress, _options);

                return new NewtonsoftRawJsonSerializerContext(_deserializer, _objectDeserializer, messageContext, messageToken, headers.GetMessageTypes(),
                    _options, ContentType);
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

        public MessageBody GetMessageBody(string text)
        {
            return new StringMessageBody(text);
        }
    }
}
