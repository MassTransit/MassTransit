namespace MassTransit.Transformation
{
    using System;
    using System.Threading.Tasks;
    using Initializers;
    using Util;


    /// <summary>
    /// Copies the input property, as-is, for the property value
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class DelegatePropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IPropertyProvider<TInput, TProperty> _inputProvider;
        readonly Func<TransformPropertyContext<TProperty, TInput>, Task<TProperty>> _valueProvider;

        public DelegatePropertyProvider(IPropertyProvider<TInput, TProperty> inputProvider,
            Func<TransformPropertyContext<TProperty, TInput>, Task<TProperty>> valueProvider)
        {
            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));

            _inputProvider = inputProvider;
            _valueProvider = valueProvider;
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (!context.TryGetPayload(out TransformContext<TInput> transformContext))
                return TaskUtil.Default<TProperty>();

            if (!context.HasInput)
                return TaskUtil.Default<TProperty>();

            Task<TProperty> inputTask = _inputProvider.GetProperty(context);
            if (inputTask.IsCompleted)
            {
                var propertyContext = new MessageTransformPropertyContext<TProperty, TInput>(transformContext, inputTask.Result);

                return _valueProvider(propertyContext);
            }

            async Task<TProperty> GetPropertyAsync()
            {
                var inputValue = await inputTask.ConfigureAwait(false);
                var propertyContext = new MessageTransformPropertyContext<TProperty, TInput>(transformContext, inputValue);

                return await _valueProvider(propertyContext).ConfigureAwait(false);
            }

            return GetPropertyAsync();
        }
    }
}
