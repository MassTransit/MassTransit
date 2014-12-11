namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ValueValueDictionaryObjectMapper<T, TKey, TValue> :
        IObjectMapper<T>
    {
        readonly ReadWriteProperty<T> _property;

        public ValueValueDictionaryObjectMapper(ReadWriteProperty<T> property)
        {
            _property = property;
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            IArrayValueProvider values;
            if (!valueProvider.TryGetValue(_property.Property.Name, out values))
                return;

            var elements = new Dictionary<TKey, TValue>();

            for (int i = 0;; i++)
            {
                IArrayValueProvider elementArray;
                if (!values.TryGetValue(i, out elementArray))
                    break;

                TKey elementKey;
                if (elementArray.TryGetValue(0, out elementKey))
                {
                    TValue elementValue;
                    elementArray.TryGetValue(1, out elementValue);

                    elements[elementKey] = elementValue;
                }
            }

            _property.Set(obj, elements);
        }
    }
}