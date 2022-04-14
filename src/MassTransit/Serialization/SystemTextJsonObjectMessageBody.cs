#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.Json;


    public class SystemTextJsonObjectMessageBody<T> :
        MessageBody
        where T : class
    {
        readonly JsonSerializerOptions _options;
        readonly T _value;
        byte[]? _bytes;
        string? _string;

        public SystemTextJsonObjectMessageBody(T value, JsonSerializerOptions options)
        {
            _value = value;
            _options = options;
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
                _bytes = JsonSerializer.SerializeToUtf8Bytes(_value, _options);

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
                _string = JsonSerializer.Serialize(_value, _options);

                return _string;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }
    }
}
