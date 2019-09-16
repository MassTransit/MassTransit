namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using GreenPipes;
    using Newtonsoft.Json;


    public class JsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;

        public JsonMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", JsonMessageSerializer.JsonContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => JsonMessageSerializer.JsonContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                var messageEncoding = GetMessageEncoding(receiveContext);

                MessageEnvelope envelope;
                using (var body = receiveContext.GetBodyStream())
                using (var reader = new StreamReader(body, messageEncoding, false, 1024, true))
                using (var jsonReader = new JsonTextReader(reader))
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

        static Encoding GetMessageEncoding(ReceiveContext receiveContext)
        {
            var contentEncoding = receiveContext.TransportHeaders.Get("Content-Encoding", default(string));

            return string.IsNullOrWhiteSpace(contentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(contentEncoding);
        }
    }
}
