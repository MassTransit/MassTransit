namespace MassTransit.Initializers.PropertyProviders
{
    using System.Threading.Tasks;


    /// <summary>
    /// Returns a constant value for the property
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class ConstantPropertyProvider<TInput, TProperty> :
        IPropertyProvider<TInput, TProperty>
        where TInput : class
    {
        readonly Task<TProperty> _propertyValue;

        public ConstantPropertyProvider(TProperty propertyValue)
        {
            _propertyValue = Task.FromResult(propertyValue);
        }

        public Task<TProperty> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            return _propertyValue;
        }
    }
}
