#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text.Json;


    public class SystemTextJsonRawMessageSerializer :
        RawMessageSerializer,
        IMessageDeserializer,
        IMessageSerializer
    {
        public static readonly ContentType JsonContentType = new ContentType("application/json");

        readonly RawSerializerOptions _options;

        public SystemTextJsonRawMessageSerializer(RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _options = options;
        }

        public ContentType ContentType => JsonContentType;

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", ContentType.MediaType);
            scope.Add("provider", "System.Text.Json");
        }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(body.GetBytes(), SystemTextJsonMessageSerializer.Options);

                var messageTypes = headers.GetMessageTypes();

                var messageContext = new RawMessageContext(headers, destinationAddress, _options);

                var serializerContext = new SystemTextJsonRawSerializerContext(SystemTextJsonMessageSerializer.Instance,
                    SystemTextJsonMessageSerializer.Options, ContentType, messageContext, messageTypes, _options, jsonElement);

                return serializerContext;
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An error occured while deserializing the message enveloper", ex);
            }
        }

        public MessageBody GetMessageBody(string text)
        {
            return new StringMessageBody(text);
        }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            if (_options.HasFlag(RawSerializerOptions.AddTransportHeaders))
                SetRawMessageHeaders<T>(context);

            return new SystemTextJsonRawMessageBody<T>(context, SystemTextJsonMessageSerializer.Options);
        }
    }
}
