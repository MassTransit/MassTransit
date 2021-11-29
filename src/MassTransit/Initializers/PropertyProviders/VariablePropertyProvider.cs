namespace MassTransit.Initializers.PropertyProviders
{
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Copies the input property, as-is, for the property value
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class VariablePropertyProvider<TInput, TProperty, TValue> :
        IPropertyProvider<TInput, TValue>
        where TInput : class
        where TProperty : class, IInitializerVariable<TValue>
    {
        readonly IPropertyProvider<TInput, TProperty> _provider;

        public VariablePropertyProvider(IPropertyProvider<TInput, TProperty> provider)
        {
            _provider = provider;
        }

        public Task<TValue> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (!context.HasInput)
                return TaskUtil.Default<TValue>();

            Task<TProperty> propertyTask = _provider.GetProperty(context);
            if (propertyTask.Status == TaskStatus.RanToCompletion)
                return propertyTask.Result.GetValue(context);

            async Task<TValue> GetPropertyAsync()
            {
                var property = await propertyTask.ConfigureAwait(false);

                return await property.GetValue(context).ConfigureAwait(false);
            }

            return GetPropertyAsync();
        }
    }
}
