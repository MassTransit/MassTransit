namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class PropertyConverterPropertyProvider<TInput, TProperty, TInputProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IPropertyConverter<TProperty, TInputProperty> _converter;
        readonly IPropertyProvider<TInput, TInputProperty> _inputProvider;

        public PropertyConverterPropertyProvider(IPropertyConverter<TProperty, TInputProperty> converter,
            IPropertyProvider<TInput, TInputProperty> inputProvider)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));

            _converter = converter;
            _inputProvider = inputProvider;
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            var inputTask = _inputProvider.GetProperty(context);
            if (inputTask.IsCompleted)
                return context.HasInput
                    ? _converter.Convert(context, inputTask.Result)
                    : TaskUtil.Default<TProperty>();

            return GetPropertyAsync(context, inputTask);
        }

        async Task<TProperty> GetPropertyAsync<T>(InitializeContext<T, TInput> context, Task<TInputProperty> inputTask)
            where T : class
        {
            var inputValue = await inputTask.ConfigureAwait(false);

            return context.HasInput
                ? await _converter.Convert(context, inputValue).ConfigureAwait(false)
                : default;
        }
    }
}
