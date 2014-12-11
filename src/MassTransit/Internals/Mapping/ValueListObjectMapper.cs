namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;
    using Reflection;


    public class ValueListObjectMapper<T, TElement> :
        IObjectMapper<T>
    {
        readonly ReadWriteProperty<T> _property;

        public ValueListObjectMapper(ReadWriteProperty<T> property)
        {
            _property = property;
        }

        public void ApplyTo(T obj, IObjectValueProvider valueProvider)
        {
            IArrayValueProvider values;
            if (!valueProvider.TryGetValue(_property.Property.Name, out values))
                return;

            var elements = new List<TElement>();

            for (int i = 0;; i++)
            {
                TElement element;
                if (!values.TryGetValue(i, out element))
                    break;

                elements.Add(element);
            }

            _property.Set(obj, elements.ToArray());
        }
    }
}