#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NServiceBusJsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;

        public NServiceBusJsonMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("deserializer");
            scope.Add("contentType", NServiceBusJsonMessageSerializer.JsonContentType.MediaType);
        }

        public ContentType ContentType => NServiceBusJsonMessageSerializer.JsonContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                using var stream = body.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
                using var jsonReader = new JsonTextReader(reader);

                var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                var messageContext = new NServiceBusHeaderAdapter(headers);

                return new NewtonsoftRawJsonSerializerContext(_deserializer, _objectDeserializer, messageContext, messageToken, headers.GetMessageTypes(),
                    RawSerializerOptions.Default, ContentType);
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
