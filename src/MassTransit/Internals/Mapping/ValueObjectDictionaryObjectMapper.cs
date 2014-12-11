namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ValueObjectDictionaryObjectMapper<T, TKey, TValue> :
        IObjectMapper<T>
    {
        readonly ReadWriteProperty<T> _property;
        readonly IObjectConverter _valueConverter;

        public ValueObjectDictionaryObjectMapper(ReadWriteProperty<T> property, IObjectConverter valueConverter)
        {
            _property = property;
            _valueConverter = valueConverter;
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
                    TValue elementValue = default(TValue);
                    IObjectValueProvider elementValueProvider;
                    if (elementArray.TryGetValue(1, out elementValueProvider))
                        elementValue = (TValue)_valueConverter.GetObject(elementValueProvider);

                    elements[elementKey] = elementValue;
                }
            }

            _property.Set(obj, elements);
        }
    }
}