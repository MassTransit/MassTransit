namespace MassTransit.Initializers.PropertyInitializers
{
    using PropertyProviders;


    public class InputValuePropertyProviderFactory<TInput> :
        IPropertyProviderFactory<TInput>
        where TInput : class
    {
        readonly string _inputPropertyName;

        public InputValuePropertyProviderFactory(string inputPropertyName)
        {
            _inputPropertyName = inputPropertyName;
        }

        public IPropertyProvider<TInput, TInputProperty> CreatePropertyProvider<TInputProperty>()
        {
            return new InputValuePropertyProvider<TInput, TInputProperty>(_inputPropertyName);
        }
    }
}
