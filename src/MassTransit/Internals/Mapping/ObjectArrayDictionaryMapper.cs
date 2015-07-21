namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ObjectArrayDictionaryMapper<T, TElement> :
        IDictionaryMapper<T>
    {
        readonly IDictionaryConverter _elementConverter;
        readonly ReadOnlyProperty<T> _property;

        public ObjectArrayDictionaryMapper(ReadOnlyProperty<T> property, IDictionaryConverter elementConverter)
        {
            _property = property;
            _elementConverter = elementConverter;
        }

        public void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj)
        {
            object value = _property.Get(obj);

            var values = value as TElement[];
            if (values == null)
                return;

            var elements = new IDictionary<string, object>[values.Length];
            for (int i = 0; i < values.Length; i++)
                elements[i] = _elementConverter.GetDictionary(values[i]);

            dictionary.Add(_property.Property.Name, elements);
        }
    }
}