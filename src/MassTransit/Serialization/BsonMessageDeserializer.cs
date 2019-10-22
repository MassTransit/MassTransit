namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class BsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;

        public BsonMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("bson");
            scope.Add("contentType", BsonMessageSerializer.BsonContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => BsonMessageSerializer.BsonContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                MessageEnvelope envelope;
                using (Stream body = receiveContext.GetBodyStream())
                using (var jsonReader = new BsonDataReader(body))
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
