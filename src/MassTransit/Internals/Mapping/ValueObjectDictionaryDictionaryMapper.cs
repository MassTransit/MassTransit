namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ValueObjectDictionaryDictionaryMapper<T, TKey, TValue> :
        IDictionaryMapper<T>
    {
        readonly IDictionaryConverter _elementConverter;
        readonly ReadOnlyProperty<T> _property;

        public ValueObjectDictionaryDictionaryMapper(ReadOnlyProperty<T> property, IDictionaryConverter elementConverter)
        {
            _property = property;
            _elementConverter = elementConverter;
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
                elements.Add(new object[] {element.Key, _elementConverter.GetDictionary(element.Value)});

            dictionary.Add(_property.Property.Name, elements.ToArray());
        }
    }
}