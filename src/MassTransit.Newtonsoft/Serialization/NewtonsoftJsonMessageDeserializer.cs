#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;


    public class NewtonsoftJsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;

        public NewtonsoftJsonMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", ContentType.MediaType);
        }

        public ContentType ContentType => NewtonsoftJsonMessageSerializer.JsonContentType;

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

                var envelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);
                if (envelope == null)
                    throw new SerializationException("The message envelope was not found.");

                return new NewtonsoftJsonSerializerContext(_deserializer, _objectDeserializer, envelope, ContentType);
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException("A JSON serialization exception occurred while deserializing the message envelope", ex);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An exception occurred while deserializing the message envelope", ex);
            }
        }

        public MessageBody GetMessageBody(string text)
        {
            return new StringMessageBody(text);
        }
    }
}
