namespace MassTransit.Interop.NServiceBus.Serialization
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Contexts;
    using GreenPipes;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NServiceBusXmlMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;

        public NServiceBusXmlMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("deserializer");
            scope.Add("contentType", XmlContentType.MediaType);
        }

        public const string ContentTypeHeaderValue = "text/xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);

        ContentType IMessageDeserializer.ContentType => XmlContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                XDocument document;
                long position;
                using (var body = receiveContext.GetBodyStream())
                {
                    using (var xmlReader = XmlReader.Create(body, new XmlReaderSettings {CheckCharacters = false}))
                    {
                        document = XDocument.Load(xmlReader);

                        position = body.Position;
                    }
                }

                var json = new StringBuilder((int) position);

                using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;

                    XmlMessageSerializer.XmlSerializer.Serialize(jsonWriter, document.Root);
                }

                using (var stringReader = new StringReader(json.ToString()))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                    return new NServiceBusConsumeContext(_deserializer, receiveContext, messageToken);
                }
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