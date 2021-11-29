namespace MassTransit.Initializers.PropertyProviders
{
    using System.Threading.Tasks;
    using Util;


    public class ToNullablePropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty?>
        where TInput : class
        where TProperty : struct
    {
        readonly IPropertyProvider<TInput, TProperty> _provider;

        public ToNullablePropertyProvider(IPropertyProvider<TInput, TProperty> provider)
        {
            _provider = provider;
        }

        public Task<TProperty?> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (!context.HasInput)
                return TaskUtil.Default<TProperty?>();

            Task<TProperty> propertyTask = _provider.GetProperty(context);
            if (propertyTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<TProperty?>(propertyTask.Result);

            async Task<TProperty?> GetPropertyAsync()
            {
                return await propertyTask.ConfigureAwait(false);
            }

            return GetPropertyAsync();
        }
    }
}
