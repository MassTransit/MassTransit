#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using GrpcTransport;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Linq;


    public class GrpcMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;

        public GrpcMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", GrpcMessageSerializer.GrpcContentType.MediaType);
        }

        public ContentType ContentType => GrpcMessageSerializer.GrpcContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            var context = receiveContext as GrpcReceiveContext ?? throw new ArgumentException("Must be GrpcReceiveContext", nameof(receiveContext));

            try
            {
                using var stream = receiveContext.Body.GetStream();
                using var jsonReader = new BsonDataReader(stream);

                var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                var serializerContext = new NewtonsoftRawBsonSerializerContext(_deserializer, _objectDeserializer, context.Message, messageToken,
                    context.Message.MessageType, ContentType);

                var consumeContext = new BodyConsumeContext(receiveContext, serializerContext);

                consumeContext.AddOrUpdatePayload<GrpcConsumeContext>(() => context.Message, _ => context.Message);

                return consumeContext;
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

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                using var stream = body.GetStream();
                using var jsonReader = new BsonDataReader(stream);

                var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                var messageContext = new RawMessageContext(headers, destinationAddress, RawSerializerOptions.Default);

                // TODO this is likely broken but fix it properly vs the original hack
                return new NewtonsoftRawBsonSerializerContext(_deserializer, _objectDeserializer, messageContext, messageToken, headers.GetMessageTypes(),
                    ContentType);
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
            return new Base64MessageBody(text);
        }
    }
}
