namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public class NotDefaultValueTypeSagaQueryPropertySelector<TData, TProperty> :
        ISagaQueryPropertySelector<TData, TProperty>
        where TData : class
        where TProperty : struct
    {
        readonly Func<ConsumeContext<TData>, TProperty> _selector;

        public NotDefaultValueTypeSagaQueryPropertySelector(Func<ConsumeContext<TData>, TProperty> selector)
        {
            _selector = selector;
        }

        public bool TryGetProperty(ConsumeContext<TData> context, out TProperty property)
        {
            property = _selector(context);

            return !EqualityComparer<TProperty>.Default.Equals(property, default);
        }
    }
}
