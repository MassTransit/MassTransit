namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class EnumDictionaryMapper<T> :
        IDictionaryMapper<T>
    {
        readonly ReadOnlyProperty<T> _property;

        public EnumDictionaryMapper(ReadOnlyProperty<T> property)
        {
            _property = property;
        }

        public void WritePropertyToDictionary(IDictionary<string, object> dictionary, T obj)
        {
            object value = _property.Get(obj);
            if (value == null)
                return;

            dictionary.Add(_property.Property.Name, value);
        }
    }
}