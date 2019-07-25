namespace MassTransit.Initializers.PropertyInitializers
{
    using PropertyProviders;


    public interface IPropertyProviderFactory<in TInput>
        where TInput : class
    {
        IPropertyProvider<TInput, TInputProperty> CreatePropertyProvider<TInputProperty>();
    }
}
