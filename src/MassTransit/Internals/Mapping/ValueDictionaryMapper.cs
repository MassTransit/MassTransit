namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ValueDictionaryMapper<T> :
        IDictionaryMapper<T>
    {
        readonly ReadOnlyProperty<T> _property;

        public ValueDictionaryMapper(ReadOnlyProperty<T> property)
        {
            _property = property;
        }

        public void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj)
        {
            dictionary.Add(_property.Property.Name, _property.Get(obj));
        }
    }
}