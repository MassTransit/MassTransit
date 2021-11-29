namespace MassTransit.Configuration
{
    using System;


    public class HasValueTypeSagaQueryPropertySelector<TData, TProperty> :
        ISagaQueryPropertySelector<TData, TProperty?>
        where TData : class
        where TProperty : struct
    {
        readonly Func<ConsumeContext<TData>, TProperty?> _selector;

        public HasValueTypeSagaQueryPropertySelector(Func<ConsumeContext<TData>, TProperty?> selector)
        {
            _selector = selector;
        }

        public bool TryGetProperty(ConsumeContext<TData> context, out TProperty? property)
        {
            property = _selector(context);

            return property.HasValue;
        }
    }
}
