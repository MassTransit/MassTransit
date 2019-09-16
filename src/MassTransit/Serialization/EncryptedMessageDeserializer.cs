namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class EncryptedMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly ICryptoStreamProvider _provider;

        public EncryptedMessageDeserializer(JsonSerializer deserializer, ICryptoStreamProvider provider)
        {
            _deserializer = deserializer;
            _provider = provider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("encrypted");
            scope.Add("contentType", EncryptedMessageSerializer.EncryptedContentType.MediaType);
            _provider.Probe(scope);
        }

        public ContentType ContentType => EncryptedMessageSerializer.EncryptedContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                MessageEnvelope envelope;
                using (Stream body = receiveContext.GetBodyStream())
                using (Stream cryptoStream = _provider.GetDecryptStream(body, receiveContext))
                using (var jsonReader = new BsonDataReader(cryptoStream))
                {
                    envelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);
                }

                return new JsonConsumeContext(_deserializer, receiveContext, envelope);
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
    }
}
