namespace MassTransit.Serialization
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using GreenPipes;
    using Newtonsoft.Json;


    public class XmlMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;

        public XmlMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("xml");
            scope.Add("contentType", XmlMessageSerializer.XmlContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => XmlMessageSerializer.XmlContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                XDocument document;
                using (Stream body = receiveContext.GetBodyStream())
                using (var xmlReader = XmlReader.Create(body, new XmlReaderSettings { CheckCharacters = false }))
                    document = XDocument.Load(xmlReader);

                var json = new StringBuilder(1024);

                using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;

                    XmlMessageSerializer.XmlSerializer.Serialize(jsonWriter, document.Root);
                }

                MessageEnvelope envelope;
                using (var stringReader = new StringReader(json.ToString()))
                using (var jsonReader = new JsonTextReader(stringReader))
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
