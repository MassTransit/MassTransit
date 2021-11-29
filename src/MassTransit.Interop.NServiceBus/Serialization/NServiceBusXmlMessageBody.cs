#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Newtonsoft.Json;


    public class NServiceBusXmlMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        byte[]? _bytes;
        string? _string;

        public NServiceBusXmlMessageBody(SendContext<TMessage> context)
        {
            _context = context;
        }

        public long? Length => _bytes?.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            if (_bytes != null)
                return _bytes;

            try
            {
                using var stream = new MemoryStream();

                var json = new StringBuilder(1024);

                using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
                using (var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Newtonsoft.Json.Formatting.None })
                {
                    NServiceBusXmlMessageSerializer.JsonSerializer.Value.Serialize(jsonWriter, _context.Message, typeof(TMessage));

                    jsonWriter.Flush();
                    stringWriter.Flush();
                }

                using (var stringReader = new StringReader(json.ToString()))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    var document = (XDocument?)NServiceBusXmlMessageSerializer.XmlSerializer.Value.Deserialize(jsonReader, typeof(XDocument));

                    if (document?.Root != null)
                        document.Root.Name = typeof(TMessage).Name;

                    using (var writer = new StreamWriter(stream, MessageDefaults.Encoding, 1024, true))
                    using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { CheckCharacters = false }))
                    {
                        document?.WriteTo(xmlWriter);
                    }
                }

                _bytes = stream.ToArray();

                return _bytes;
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }

        public string GetString()
        {
            if (_string != null)
                return _string;

            _string = Encoding.UTF8.GetString(GetBytes());
            return _string;
        }
    }
}
