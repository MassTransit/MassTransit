namespace MassTransit.Initializers.PropertyProviders
{
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Awaits a <see cref="Task{TProperty}"/> property, returning the property value.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class AsyncPropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IPropertyProvider<TInput, Task<TProperty>> _provider;

        public AsyncPropertyProvider(IPropertyProvider<TInput, Task<TProperty>> provider)
        {
            _provider = provider;
        }

        Task<TProperty> IPropertyProvider<TInput, TProperty>.GetProperty<T>(InitializeContext<T, TInput> context)
        {
            if (!context.HasInput)
                return TaskUtil.Default<TProperty>();

            var propertyTask = _provider.GetProperty(context);
            if (propertyTask.IsCompleted)
            {
                var valueTask = propertyTask.Result;
                if (valueTask.IsCompleted)
                    return valueTask;
            }

            async Task<TProperty> GetPropertyAsync()
            {
                var valueTask = await propertyTask.ConfigureAwait(false);

                return await valueTask.ConfigureAwait(false);
            }

            return GetPropertyAsync();
        }
    }


    /// <summary>
    /// Awaits a <see cref="Task{TProperty}"/> property, returning the property value.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TTask"></typeparam>
    public class AsyncPropertyProvider<TInput, TProperty, TTask> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly IPropertyProvider<TInput, Task<TTask>> _provider;
        readonly IPropertyConverter<TProperty, TTask> _converter;

        public AsyncPropertyProvider(IPropertyProvider<TInput, Task<TTask>> provider, IPropertyConverter<TProperty, TTask> converter)
        {
            _provider = provider;
            _converter = converter;
        }

        Task<TProperty> IPropertyProvider<TInput, TProperty>.GetProperty<T>(InitializeContext<T, TInput> context)
        {
            if (!context.HasInput)
                return TaskUtil.Default<TProperty>();

            var propertyTask = _provider.GetProperty(context);
            if (propertyTask.IsCompleted)
            {
                var valueTask = propertyTask.Result;
                if (valueTask.IsCompleted)
                    return _converter.Convert(context, valueTask.Result);
            }

            async Task<TProperty> GetPropertyAsync()
            {
                var valueTask = await propertyTask.ConfigureAwait(false);

                var value = await valueTask.ConfigureAwait(false);

                return await _converter.Convert(context, value).ConfigureAwait(false);
            }

            return GetPropertyAsync();
        }
    }
}
