#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json.Bson;


    public class NewtonsoftBsonMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        MessageEnvelope? _envelope;
        byte[]? _bytes;

        public NewtonsoftBsonMessageBody(SendContext<TMessage> context, MessageEnvelope? envelope = null)
        {
            _context = context;
            _envelope = envelope;
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
                var envelope = _envelope ??= new JsonMessageEnvelope(_context, _context.Message, MessageTypeCache<TMessage>.MessageTypeNames);

                using var stream = new MemoryStream();
                using var jsonWriter = new BsonDataWriter(stream);

                BsonMessageSerializer.Serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

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
