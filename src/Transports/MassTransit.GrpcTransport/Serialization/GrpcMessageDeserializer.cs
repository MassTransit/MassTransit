namespace MassTransit.GrpcTransport.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using Contexts;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Linq;


    public class GrpcMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;

        public GrpcMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", GrpcMessageSerializer.GrpcContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => GrpcMessageSerializer.GrpcContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                using var body = receiveContext.GetBodyStream();
                using var jsonReader = new BsonDataReader(body);

                var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                return new GrpcConsumeContext(_deserializer, receiveContext, messageToken);
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
    }
}
