namespace MassTransit.Initializers.Factories
{
    using Conventions;


    public class PropertyInitializerInspector<TMessage, TInput, TProperty> :
        IPropertyInitializerInspector<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly string _propertyName;

        public PropertyInitializerInspector(string propertyName)
        {
            _propertyName = propertyName;
        }

        public bool Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention)
        {
            if (convention.TryGetPropertyInitializer<TMessage, TInput, TProperty>(_propertyName, out IPropertyInitializer<TMessage, TInput> initializer))
            {
                builder.Add(_propertyName, initializer);
                return true;
            }

            return false;
        }
    }
}
