namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class EncryptedMessageDeserializerV2 :
        IMessageDeserializer
    {
        readonly ICryptoStreamProviderV2 _cryptoStreamProvider;
        readonly JsonSerializer _deserializer;

        public EncryptedMessageDeserializerV2(JsonSerializer deserializer, ICryptoStreamProviderV2 cryptoStreamProvider)
        {
            _deserializer = deserializer;
            _cryptoStreamProvider = cryptoStreamProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("encrypted");
            scope.Add("contentType", ContentType.MediaType);
            _cryptoStreamProvider.Probe(scope);
        }

        public ContentType ContentType => EncryptedMessageSerializerV2.EncryptedContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                using (var body = receiveContext.GetBodyStream())
                using (var disposingCryptoStream = _cryptoStreamProvider.GetDecryptStream(body, receiveContext))
                using (var jsonReader = new BsonDataReader(disposingCryptoStream))
                {
                    var messageEnvelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);

                    return new JsonConsumeContext(_deserializer, receiveContext, messageEnvelope);
                }
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
    }
}
