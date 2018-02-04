namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;


    public class ConvertMessageInputValuePropertyProvider<TInput, TProperty, TInputProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IPropertyConverter<TProperty, TInputProperty> _converter;
        readonly IReadProperty<TInput, TInputProperty> _inputProperty;

        public ConvertMessageInputValuePropertyProvider(IPropertyConverter<TProperty, TInputProperty> converter, string inputPropertyName)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (inputPropertyName == null)
                throw new ArgumentNullException(nameof(inputPropertyName));

            _converter = converter;

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TInputProperty>(inputPropertyName);
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (context.HasInput)
            {
                var propertyValue = _inputProperty.Get(context.Input);

                return _converter.Convert(context, propertyValue);
            }

            return Task.FromResult<TProperty>(default);
        }
    }
}