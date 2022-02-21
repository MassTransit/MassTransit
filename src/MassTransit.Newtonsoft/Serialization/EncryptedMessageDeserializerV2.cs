#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class EncryptedMessageDeserializerV2 :
        IMessageDeserializer
    {
        readonly ICryptoStreamProviderV2 _cryptoStreamProvider;
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;

        public EncryptedMessageDeserializerV2(JsonSerializer deserializer, ICryptoStreamProviderV2 cryptoStreamProvider)
        {
            _deserializer = deserializer;
            _cryptoStreamProvider = cryptoStreamProvider;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("encrypted");
            scope.Add("contentType", ContentType.MediaType);
            _cryptoStreamProvider.Probe(scope);
        }

        public ContentType ContentType => EncryptedMessageSerializerV2.EncryptedContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                using var stream = body.GetStream();
                using var disposingCryptoStream = _cryptoStreamProvider.GetDecryptStream(stream, headers);
                using var jsonReader = new BsonDataReader(disposingCryptoStream);

                var envelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);

                return new NewtonsoftEncryptedV2SerializerContext(_deserializer, _objectDeserializer, envelope, ContentType, _cryptoStreamProvider);
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException(
                    "A JSON serialization exception occurred while deserializing the message envelope", ex);
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
