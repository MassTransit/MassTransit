#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> builder)
            where T : class?
        {
            var converter = new JsonValueConverter<T>();
            var comparer = new JsonValueComparer<T>();

            builder.HasConversion(converter);
            builder.Metadata.SetValueConverter(converter);
            builder.Metadata.SetValueComparer(comparer);

            return builder;
        }
    }
}
