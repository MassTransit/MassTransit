#nullable enable
namespace MassTransit.Serialization
{
    using System.Threading;


    public static class ObjectDeserializer
    {
        static IObjectDeserializer? _serializer = SystemTextJsonMessageSerializer.Instance;

        static readonly AsyncLocal<IObjectDeserializer?> _currentSerializer = new AsyncLocal<IObjectDeserializer?>();

        public static IObjectDeserializer? Default
        {
            set => _serializer = value ?? SystemTextJsonMessageSerializer.Instance;
        }

        public static IObjectDeserializer Current
        {
            set => _currentSerializer.Value = value;
        }

        public static string? Serialize(object? value)
        {
            if (value == null)
                return null;

            var serializer = _currentSerializer.Value ?? _serializer ?? throw new ConfigurationException("No JSON serializer configured");

            return serializer.SerializeObject(value).GetString();
        }

        public static T? Deserialize<T>(object? value, T? defaultValue = null)
            where T : class
        {
            switch (value)
            {
                case null:
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
            }

            var serializer = _currentSerializer.Value ?? _serializer ?? throw new ConfigurationException("No JSON serializer configured");

            return serializer.DeserializeObject<T>(value);
        }

        public static T? Deserialize<T>(object? value, T? defaultValue = default)
            where T : struct
        {
            switch (value)
            {
                case null:
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
            }

            var serializer = _currentSerializer.Value ?? _serializer ?? throw new ConfigurationException("No JSON serializer configured");

            return serializer.DeserializeObject<T>(value);
        }
    }
}
