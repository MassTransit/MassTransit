namespace MassTransit.EntityFrameworkCoreIntegration.JobService
{
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


    public class JsonValueConverter<T> :
        ValueConverter<T, string>
        where T : class
    {
        public JsonValueConverter(ConverterMappingHints hints = default)
            : base(v => JsonHelper.Serialize(v), v => JsonHelper.Deserialize<T>(v), hints)
        {
        }
    }
}
