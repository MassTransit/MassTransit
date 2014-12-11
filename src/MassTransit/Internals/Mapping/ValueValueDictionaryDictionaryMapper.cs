namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ValueValueDictionaryDictionaryMapper<T, TKey, TValue> :
        IDictionaryMapper<T>
    {
        readonly ReadOnlyProperty<T> _property;

        public ValueValueDictionaryDictionaryMapper(ReadOnlyProperty<T> property)
        {
            _property = property;
        }

        public void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj)
        {
            object value = _property.Get(obj);
            if (value == null)
                return;

            var values = value as IDictionary<TKey, TValue>;
            if (values == null)
                return;

            var elements = new List<object>();
            foreach (var element in values)
                elements.Add(new object[] {element.Key, element.Value});

            dictionary.Add(_property.Property.Name, elements.ToArray());
        }
    }
}