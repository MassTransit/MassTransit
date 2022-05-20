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
    using Newtonsoft.Json.Linq;


    public class NewtonsoftRawXmlMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;
        readonly RawSerializerOptions _options;

        public NewtonsoftRawXmlMessageDeserializer(JsonSerializer deserializer, RawSerializerOptions options = RawSerializerOptions.Default)
        {
            _deserializer = deserializer;
            _options = options;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("xml");
            scope.Add("contentType", RawXmlMessageSerializer.RawXmlContentType.MediaType);
        }

        public ContentType ContentType => RawXmlMessageSerializer.RawXmlContentType;

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
                using (var reader = new StreamReader(stream, MessageDefaults.Encoding, false, 1024, true))
                using (var xmlReader = XmlReader.Create(reader, new XmlReaderSettings { CheckCharacters = false }))
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
                    var messageToken = _deserializer.Deserialize<JToken>(jsonReader);

                    var messageContext = new RawMessageContext(headers, destinationAddress, _options);

                    return new NewtonsoftRawXmlSerializerContext(_deserializer, _objectDeserializer, messageContext, messageToken, headers.GetMessageTypes(),
                        _options, ContentType);
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

        public MessageBody GetMessageBody(string text)
        {
            return new StringMessageBody(text);
        }
    }
}
