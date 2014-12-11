namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class NullableValueDictionaryMapper<T, TValue> :
        IDictionaryMapper<T>
        where TValue : struct
    {
        readonly ReadOnlyProperty<T> _property;

        public NullableValueDictionaryMapper(ReadOnlyProperty<T> property)
        {
            _property = property;
        }

        public void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj)
        {
            object value = _property.Get(obj);
            if (value == null)
                return;

            var nullableValue = (TValue?)value;
            dictionary.Add(_property.Property.Name, nullableValue.Value);
        }
    }
}