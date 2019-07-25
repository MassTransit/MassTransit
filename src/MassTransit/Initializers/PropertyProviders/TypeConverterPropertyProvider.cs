namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class TypeConverterPropertyProvider<TInput, TProperty, TInputProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly ITypeConverter<TProperty, TInputProperty> _converter;
        readonly IPropertyProvider<TInput, TInputProperty> _inputProvider;

        public TypeConverterPropertyProvider(ITypeConverter<TProperty, TInputProperty> converter,
            IPropertyProvider<TInput, TInputProperty> inputProvider)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            _converter = converter;
            _inputProvider = inputProvider;
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            var inputTask = _inputProvider.GetProperty(context);
            if (inputTask.IsCompleted)
                return context.HasInput && _converter.TryConvert(inputTask.Result, out var result)
                    ? Task.FromResult(result)
                    : TaskUtil.Default<TProperty>();

            return GetPropertyAsync(context, inputTask);
        }

        async Task<TProperty> GetPropertyAsync<T>(InitializeContext<T, TInput> context, Task<TInputProperty> inputTask)
            where T : class
        {
            var inputValue = await inputTask.ConfigureAwait(false);

            return context.HasInput && _converter.TryConvert(inputValue, out var result)
                ? result
                : default;
        }
    }
}
