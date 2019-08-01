namespace MassTransit.Initializers.PropertyInitializers
{
    using System.Reflection;
    using PropertyProviders;


    public class InputValuePropertyProviderFactory<TInput> :
        IPropertyProviderFactory<TInput>
        where TInput : class
    {
        readonly PropertyInfo _propertyInfo;

        public InputValuePropertyProviderFactory(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public IPropertyProvider<TInput, TInputProperty> CreatePropertyProvider<TInputProperty>()
        {
            return new InputValuePropertyProvider<TInput, TInputProperty>(_propertyInfo);
        }
    }
}
