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
    using Newtonsoft.Json.Linq;


    public class RawXmlMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly RawSerializerOptions _options;

        public RawXmlMessageDeserializer(JsonSerializer deserializer, RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _deserializer = deserializer;
            _options = options;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("xml");
            scope.Add("contentType", RawXmlMessageSerializer.RawXmlContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => RawXmlMessageSerializer.RawXmlContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                var messageEncoding = GetMessageEncoding(receiveContext);

                XDocument document;
                using (var body = receiveContext.GetBodyStream())
                using (var reader = new StreamReader(body, messageEncoding, false, 1024, true))
                using (var xmlReader = XmlReader.Create(reader, new XmlReaderSettings { CheckCharacters = false }))
                {
                    document = XDocument.Load(xmlReader);
                }

                var json = new StringBuilder(1024);

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

                    return new RawConsumeContext(_deserializer, receiveContext, messageToken, _options);
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

        static Encoding GetMessageEncoding(ReceiveContext receiveContext)
        {
            var contentEncoding = receiveContext.TransportHeaders.Get("Content-Encoding", default(string));

            return string.IsNullOrWhiteSpace(contentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(contentEncoding);
        }
    }
}
