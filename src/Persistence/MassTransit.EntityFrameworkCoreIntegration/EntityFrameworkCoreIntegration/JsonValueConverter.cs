namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text.Json;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using Serialization;


    public class JsonValueConverter<T> :
        ValueConverter<T, string>
        where T : class
    {
        public JsonValueConverter(ConverterMappingHints hints = default)
            : base(v => Serialize(v), v => Deserialize(v), hints)
        {
        }

        static T Deserialize(string json)
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<T>(json, SystemTextJsonMessageSerializer.Options);
        }

        static string Serialize(T obj)
        {
            return obj == null ? null : JsonSerializer.Serialize(obj, SystemTextJsonMessageSerializer.Options);
        }
    }
}
