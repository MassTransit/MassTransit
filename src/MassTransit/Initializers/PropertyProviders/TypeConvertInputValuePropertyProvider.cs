namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


    public class TypeConvertInputValuePropertyProvider<TInput, TProperty, TInputProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly ITypeConverter<TProperty, TInputProperty> _converter;
        readonly IReadProperty<TInput, TInputProperty> _inputProperty;

        public TypeConvertInputValuePropertyProvider(ITypeConverter<TProperty, TInputProperty> converter, string propertyName)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            _converter = converter;

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TInputProperty>(propertyName);
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            return context.HasInput && _converter.TryConvert(_inputProperty.Get(context.Input), out var result)
                ? Task.FromResult(result)
                : TaskUtil.Default<TProperty>();
        }
    }
}
