namespace MassTransit.Interop.NServiceBus.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using Contexts;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NServiceBusJsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;

        public NServiceBusJsonMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("deserializer");
            scope.Add("contentType", NServiceBusJsonMessageSerializer.JsonContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => NServiceBusJsonMessageSerializer.JsonContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                using var body = receiveContext.GetBodyStream();
                using var reader = new StreamReader(body, Encoding.UTF8, false, 1024, true);
                using var jsonReader = new JsonTextReader(reader);

                var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                return new NServiceBusConsumeContext(_deserializer, receiveContext, messageToken);
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