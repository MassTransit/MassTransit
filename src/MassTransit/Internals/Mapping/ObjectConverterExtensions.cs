namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;


    public static class ObjectConverterExtensions
    {
        public static object GetObject(this IObjectConverter converter, IDictionary<string, object> dictionary)
        {
            return converter.GetObject(new DictionaryObjectValueProvider(dictionary));
        }
    }
}