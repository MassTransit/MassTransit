#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NServiceBusXmlMessageDeserializer :
        IMessageDeserializer
    {
        public const string ContentTypeHeaderValue = "text/xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);
        readonly JsonSerializer _deserializer;
        readonly IObjectDeserializer _objectDeserializer;

        public NServiceBusXmlMessageDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
            _objectDeserializer = new NewtonsoftObjectDeserializer(deserializer);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("deserializer");
            scope.Add("contentType", XmlContentType.MediaType);
        }

        public ContentType ContentType => XmlContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                XDocument document;
                long position;
                using (var stream = body.GetStream())
                {
                    using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { CheckCharacters = false }))
                    {
                        document = XDocument.Load(xmlReader);

                        position = stream.Position;
                    }
                }

                XNamespace ns = "http://www.w3.org/2001/XMLSchema-instance";
                document.Descendants()
                    .Where(node => (string)node.Attribute(ns + "nil")! == "true")
                    .Remove();

                var json = new StringBuilder((int)position);

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

                    var messageContext = new NServiceBusHeaderAdapter(headers);

                    return new NewtonsoftRawXmlSerializerContext(_deserializer, _objectDeserializer, messageContext, messageToken,
                        messageContext.SupportedMessageTypes, RawSerializerOptions.AddTransportHeaders, ContentType);
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
