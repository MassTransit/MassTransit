#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.Json;


    public class SystemTextJsonMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;
        readonly JsonSerializerOptions _options;
        byte[]? _bytes;
        MessageEnvelope? _envelope;
        string? _string;

        public SystemTextJsonMessageBody(SendContext<TMessage> context, JsonSerializerOptions options, MessageEnvelope? envelope = null)
        {
            _context = context;
            _options = options;
            _envelope = envelope;
        }

        public long? Length => _bytes?.Length ?? _string?.Length;

        public Stream GetStream()
        {
            return new MemoryStream(GetBytes(), false);
        }

        public byte[] GetBytes()
        {
            if (_bytes != null)
                return _bytes;

            if (_string != null)
            {
                _bytes = Encoding.UTF8.GetBytes(_string);
                return _bytes;
            }

            try
            {
                var envelope = _envelope ??= new JsonMessageEnvelope(_context, _context.Message, MessageTypeCache<TMessage>.MessageTypeNames);

                _bytes = JsonSerializer.SerializeToUtf8Bytes(envelope, _options);

                return _bytes;
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

            if (_bytes != null)
            {
                _string = Encoding.UTF8.GetString(_bytes);
                return _string;
            }

            try
            {
                var envelope = _envelope ??= new JsonMessageEnvelope(_context, _context.Message, MessageTypeCache<TMessage>.MessageTypeNames);

                _string = JsonSerializer.Serialize(envelope, _options);

                return _string;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }
    }
}
