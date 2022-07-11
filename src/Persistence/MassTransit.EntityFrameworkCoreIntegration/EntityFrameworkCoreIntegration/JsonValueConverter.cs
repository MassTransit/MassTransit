namespace MassTransit.EntityFrameworkCoreIntegration
{
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
            return ObjectDeserializer.Deserialize<T>(json);
        }

        static string Serialize(T obj)
        {
            return ObjectDeserializer.Serialize(obj);
        }
    }
}
