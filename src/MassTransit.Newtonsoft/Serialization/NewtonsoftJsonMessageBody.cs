#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using Newtonsoft.Json;


    public class NewtonsoftJsonMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        byte[]? _bytes;
        MessageEnvelope? _envelope;
        string? _string;

        public NewtonsoftJsonMessageBody(SendContext<TMessage> context, MessageEnvelope? envelope = null)
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
                using var writer = new StreamWriter(stream, MessageDefaults.Encoding, 1024, true);
                using var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented };

                NewtonsoftJsonMessageSerializer.Serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

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
