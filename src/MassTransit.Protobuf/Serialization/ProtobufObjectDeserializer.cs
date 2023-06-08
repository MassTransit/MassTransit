#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using Initializers;
    using Initializers.TypeConverters;
    using MassTransit.Metadata;
    using ProtoBuf.Meta;

    public class ProtobufObjectDeserializer : IObjectDeserializer
    {
        private readonly RuntimeTypeModel _typeModel;

        public ProtobufObjectDeserializer(RuntimeTypeModel typeModel)
        {
            _typeModel = typeModel;
        }

        public T? DeserializeObject<T>(object? value, T? defaultValue = default)
            where T : class
        {
            switch (value)
            {
                case null:
                    return defaultValue;
                case T headerValue:
                    return headerValue;
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
                case string json when typeof(T).IsInterface && TypeMetadataCache<T>.IsValidMessageType:
                    using (var stream = new MemoryStream())
                    {
                        var bytes = Convert.FromBase64String(json);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Position = 0;
                        return _typeModel.Deserialize<T>(stream);
                    }
                case string text when TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter)
                                   && typeConverter.TryConvert(text, out var result):
                    return result;
            }

            throw new InvalidOperationException($"Unsupported deserialization type: {typeof(T)}");
        }

        public T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : struct
        {
            switch (value)
            {
                case null:
                    return defaultValue;
                case T headerValue:
                    return headerValue;
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
                case string text when TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter)
                                   && typeConverter.TryConvert(text, out var result):
                    return result;
            }

            throw new InvalidOperationException($"Unsupported deserialization type: {typeof(T)}");
        }

        public MessageBody SerializeObject(object? value)
        {
            if (value == null)
                return new EmptyMessageBody();

            using (var stream = new MemoryStream())
            {
                _typeModel.Serialize(stream, value);
                stream.Position = 0;
                var bytes = stream.ToArray();
                var base64String = Convert.ToBase64String(bytes);
                return new StringMessageBody(base64String);
            }
        }
    }

}
