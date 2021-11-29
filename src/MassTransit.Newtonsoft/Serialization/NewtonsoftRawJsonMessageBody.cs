#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NewtonsoftRawJsonMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        readonly JToken? _message;
        byte[]? _bytes;
        string? _string;

        public NewtonsoftRawJsonMessageBody(SendContext<TMessage> context, JToken? message = null)
        {
            _context = context;
            _message = message;
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
                using var writer = new StreamWriter(stream, MessageDefaults.Encoding, 1024, true);
                using var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented };

                if (_message != null)
                    NewtonsoftJsonMessageSerializer.Serializer.Serialize(jsonWriter, _message);
                else
                    NewtonsoftJsonMessageSerializer.Serializer.Serialize(jsonWriter, _context.Message, typeof(TMessage));

                jsonWriter.Flush();
                writer.Flush();

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
