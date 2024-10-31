namespace MassTransit.AzureTable.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;


    public static class EntityConverterFactory
    {
        public static IEntityConverter<T> CreateConverter<T>()
            where T : class
        {
            List<IEntityPropertyConverter<T>> converters = MessageTypeCache<T>.Properties.Where(x => x.CanRead && x.CanWrite)
                .Select(x => CreatePropertyConverter<T>(x))
                .ToList();

            return new EntityConverter<T>(converters);
        }

        static IEntityPropertyConverter<T> CreatePropertyConverter<T>(PropertyInfo propertyInfo)
            where T : class
        {
            if (EntityPropertyTypeConverter.IsSupported(propertyInfo.PropertyType))
            {
                return (IEntityPropertyConverter<T>)Activator.CreateInstance(
                    typeof(EntityPropertyConverter<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType), propertyInfo.Name);
            }

            if (propertyInfo.PropertyType.IsValueType)
            {
                return (IEntityPropertyConverter<T>)Activator.CreateInstance(
                    typeof(ValueTypeEntityPropertyConverter<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType), propertyInfo.Name);
            }

            return (IEntityPropertyConverter<T>)Activator.CreateInstance(
                typeof(ObjectEntityPropertyConverter<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType), propertyInfo.Name);
        }
    }
}
