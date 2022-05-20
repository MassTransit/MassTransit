#nullable enable
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
    using Newtonsoft.Json;


    public class NewtonsoftXmlMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;

        public NewtonsoftXmlMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("xml");
            scope.Add("contentType", NewtonsoftXmlMessageSerializer.XmlContentType.MediaType);
        }

        public ContentType ContentType => NewtonsoftXmlMessageSerializer.XmlContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                XDocument document;
                using (var stream = body.GetStream())
                using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { CheckCharacters = false }))
                {
                    document = XDocument.Load(xmlReader);
                }

                var json = new StringBuilder(1024);

                using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;

                    NewtonsoftXmlMessageSerializer.XmlSerializer.Serialize(jsonWriter, document.Root);
                }

                using (var stringReader = new StringReader(json.ToString()))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    var envelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);
                    if (envelope == null)
                        throw new SerializationException("The message envelope was not found.");

                    return new NewtonsoftXmlSerializerContext(_deserializer, _objectDeserializer, envelope, ContentType);
                }
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

        public MessageBody GetMessageBody(string text)
        {
            return new StringMessageBody(text);
        }
    }
}
