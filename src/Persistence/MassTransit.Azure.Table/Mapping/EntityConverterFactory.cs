namespace MassTransit.Azure.Table.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Metadata;


    public static class EntityConverterFactory
    {
        public static IEntityConverter<T> CreateConverter<T>()
            where T : class
        {
            List<IEntityPropertyConverter<T>> converters = TypeMetadataCache<T>.Properties.Where(x => x.CanRead && x.CanWrite)
                .Select(x => (IEntityPropertyConverter<T>)Activator.CreateInstance(
                    typeof(EntityPropertyConverter<,>).MakeGenericType(typeof(T), x.PropertyType), x.Name))
                .ToList();

            return new EntityConverter<T>(converters);
        }
    }
}
