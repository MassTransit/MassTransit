#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Linq;


    public class NewtonsoftRawBsonMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        readonly JToken? _message;
        byte[]? _bytes;

        public NewtonsoftRawBsonMessageBody(SendContext<TMessage> context, JToken? message = null)
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
                using var jsonWriter = new BsonDataWriter(stream);

                if (_message != null)
                    BsonMessageSerializer.Serializer.Serialize(jsonWriter, _message);
                else
                    BsonMessageSerializer.Serializer.Serialize(jsonWriter, _context.Message, typeof(TMessage));

                jsonWriter.Flush();

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
            return Convert.ToBase64String(GetBytes());
        }
    }
}
