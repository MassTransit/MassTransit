namespace MassTransit.Configuration
{
    using System;


    public class SagaQueryPropertySelector<TData, TProperty> :
        ISagaQueryPropertySelector<TData, TProperty>
        where TData : class
        where TProperty : class
    {
        readonly Func<ConsumeContext<TData>, TProperty> _selector;

        public SagaQueryPropertySelector(Func<ConsumeContext<TData>, TProperty> selector)
        {
            _selector = selector;
        }

        public bool TryGetProperty(ConsumeContext<TData> context, out TProperty property)
        {
            property = _selector(context);

            return property != null;
        }
    }
}
