namespace MassTransit.Serialization
{
    using GreenPipes;
    using MassTransit.Metadata;
    using MassTransit.Util;
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text.Json;

    public class SystemTextJsonMessageDeserializer : IMessageDeserializer
    {
        public ContentType ContentType => SystemTextJsonMessageSerializer.JsonContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                var envelope = DeserializerEnvelope(receiveContext);
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

        private static MessageEnvelope DeserializerEnvelope(ReceiveContext receiveContext)
        {
            using var body = receiveContext.GetBodyStream();

            return JsonSerializer.DeserializeAsync<SystemTextJsonMessageEnvelope>(body, SystemTextJsonConfiguration.Options)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

    }
}
