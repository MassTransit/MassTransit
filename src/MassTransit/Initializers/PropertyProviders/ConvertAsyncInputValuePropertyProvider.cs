namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;


    public class ConvertAsyncInputValuePropertyProvider<TInput, TProperty, TInputProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly ITypeConverter<TProperty, TInputProperty> _converter;
        readonly IReadProperty<TInput, Task<TInputProperty>> _inputProperty;

        public ConvertAsyncInputValuePropertyProvider(ITypeConverter<TProperty, TInputProperty> converter, string propertyName)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            _converter = converter;

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<Task<TInputProperty>>(propertyName);
        }

        public async Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            return context.HasInput && _converter.TryConvert(await _inputProperty.Get(context.Input).ConfigureAwait(false), out var result)
                ? result
                : default;
        }
    }
}