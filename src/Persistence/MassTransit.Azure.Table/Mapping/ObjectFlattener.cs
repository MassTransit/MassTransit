namespace MassTransit.Azure.Table.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;

    public static class ObjectFlattener
    {
        public static Dictionary<string, EntityProperty> Flatten<T>(T entity)
        {
            Dictionary<string, EntityProperty> propertyDictionary = new Dictionary<string, EntityProperty>();

            var type = typeof(T);
            IEnumerable<PropertyInfo> properties = GetProperties(type);

            foreach (var prop in properties)
            {
                var val = prop.GetValue(entity);
                EntityProperty toAdd = val switch
                {
                    string x => new EntityProperty(x),
                    byte[] x => new EntityProperty(x),
                    bool x => new EntityProperty(x),
                    DateTime x => new EntityProperty(x),
                    double x => new EntityProperty(x),
                    Guid x => new EntityProperty(x),
                    int x => new EntityProperty(x),
                    long x => new EntityProperty(x),
                    _ => new EntityProperty(JsonConvert.SerializeObject(val))
                };
                propertyDictionary.Add(prop.Name, toAdd);
            }

            return propertyDictionary;
        }

        public static T ConvertBack<T>(IDictionary<string, EntityProperty> entity)
        {
            var instance = (T) Activator.CreateInstance(typeof (T));
            var entityType = typeof(T);
            IEnumerable<PropertyInfo> properties = GetProperties(entityType);
            foreach (var prop in properties)
            {
                EntityProperty keyVal;
                if (entity.TryGetValue(prop.Name, out keyVal))
                {
                    var val = keyVal.PropertyType switch
                    {
                        EdmType.String => ExtractStringValue(prop, keyVal),
                        EdmType.Binary => keyVal.BinaryValue,
                        EdmType.Boolean => keyVal.BooleanValue,
                        EdmType.DateTime => keyVal.DateTime,
                        EdmType.Double => keyVal.DoubleValue,
                        EdmType.Guid => keyVal.GuidValue,
                        EdmType.Int32 => keyVal.Int32Value,
                        EdmType.Int64 => keyVal.Int64Value,
                        _ => null
                    };
                    prop.SetValue(instance, val);
                }
            }

            return instance;
        }

        static object ExtractStringValue(PropertyInfo prop, EntityProperty keyVal)
        {
            return prop.PropertyType == typeof(string) ? keyVal.StringValue : JsonConvert.DeserializeObject(keyVal.StringValue, prop.PropertyType);
        }

        static IEnumerable<PropertyInfo> GetProperties(IReflect type)
        {
            List<PropertyInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite)
                .ToList();
            return properties;
        }
    }
}
