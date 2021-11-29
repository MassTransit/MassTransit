#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.Json;


    public class SystemTextJsonRawMessageBody<TMessage> :
        MessageBody
        where TMessage : class
    {
        readonly object? _message;
        readonly JsonSerializerOptions _options;
        byte[]? _bytes;
        string? _string;

        public SystemTextJsonRawMessageBody(SendContext<TMessage> context, JsonSerializerOptions options, object? message = null)
        {
            _options = options;
            _message = message ?? context.Message;
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
                _bytes = JsonSerializer.SerializeToUtf8Bytes(_message, _options);

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
                _string = JsonSerializer.Serialize(_message, _options);

                return _string;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }
    }
}
