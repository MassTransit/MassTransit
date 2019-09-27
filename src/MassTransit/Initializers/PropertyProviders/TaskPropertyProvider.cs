namespace MassTransit.Initializers.PropertyProviders
{
    using System.Threading.Tasks;
    using Util;


    public class TaskPropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, Task<TProperty>>
        where TInput : class
    {
        readonly IPropertyProvider<TInput, TProperty> _provider;

        public TaskPropertyProvider(IPropertyProvider<TInput, TProperty> provider)
        {
            _provider = provider;
        }

        public Task<Task<TProperty>> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            return Task.FromResult(context.HasInput
                ? _provider.GetProperty(context)
                : TaskUtil.Default<TProperty>());
        }
    }
}
