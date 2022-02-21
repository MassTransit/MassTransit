#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class EncryptedMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;
        readonly ICryptoStreamProvider _provider;

        public EncryptedMessageDeserializer(JsonSerializer deserializer, ICryptoStreamProvider provider)
        {
            _deserializer = deserializer;
            _provider = provider;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("encrypted");
            scope.Add("contentType", EncryptedMessageSerializer.EncryptedContentType.MediaType);
            _provider.Probe(scope);
        }

        public ContentType ContentType => EncryptedMessageSerializer.EncryptedContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                using var stream = body.GetStream();
                using var cryptoStream = _provider.GetDecryptStream(stream, headers);
                using var jsonReader = new BsonDataReader(cryptoStream);

                var envelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);
                if (envelope == null)
                    throw new SerializationException("The message envelope was not found.");

                return new NewtonsoftEncryptedSerializerContext(_deserializer, _objectDeserializer, envelope, ContentType, _provider);
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
            return new Base64MessageBody(text);
        }
    }
}
