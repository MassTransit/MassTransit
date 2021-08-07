namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using GreenPipes;


    public class SystemTextJsonMessageDeserializer :
        IMessageDeserializer
    {
        public SystemTextJsonMessageDeserializer(ContentType contentType = null)
        {
            ContentType = contentType ?? SystemTextJsonMessageSerializer.JsonContentType;
        }

        public ContentType ContentType { get; }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                var envelope = JsonSerializer.Deserialize<MessageEnvelope>(receiveContext.GetBody(), SystemTextJsonMessageSerializer.Options);

                return new SystemTextJsonConsumeContext(receiveContext, envelope);
            }
            catch (Exception ex)
            {
                throw new SerializationException("An error occured while deserializing the message enveloper", ex);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", ContentType.MediaType);
        }
    }
}
