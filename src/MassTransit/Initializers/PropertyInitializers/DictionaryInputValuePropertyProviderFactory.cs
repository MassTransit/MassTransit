namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using PropertyProviders;


    public class DictionaryInputValuePropertyProviderFactory<TInput> :
        IPropertyProviderFactory<TInput>
        where TInput : class
    {
        readonly string _key;

        public DictionaryInputValuePropertyProviderFactory(string key)
        {
            _key = key;
        }

        public IPropertyProvider<TInput, TInputProperty> CreatePropertyProvider<TInputProperty>()
        {
            var providerType = typeof(InputDictionaryValuePropertyProvider<,>).MakeGenericType(typeof(TInput), typeof(TInputProperty));

            return (IPropertyProvider<TInput, TInputProperty>)Activator.CreateInstance(providerType, _key);
        }
    }
}