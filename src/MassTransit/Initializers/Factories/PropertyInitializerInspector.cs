namespace MassTransit.Initializers.Factories
{
    using System.Reflection;
    using Conventions;
    using Internals;


    public class PropertyInitializerInspector<TMessage, TInput, TProperty> :
        IPropertyInitializerInspector<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly PropertyInfo _propertyInfo;

        public PropertyInitializerInspector(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public bool Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention)
        {
            if (builder.IsInputPropertyUsed(_propertyInfo.Name))
                return false;

            if (!WritePropertyCache<TMessage>.CanWrite(_propertyInfo.Name))
                return false;

            if (convention.TryGetPropertyInitializer<TMessage, TInput, TProperty>(_propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer))
            {
                builder.Add(_propertyInfo.Name, initializer);

                builder.SetInputPropertyUsed(_propertyInfo.Name);
                return true;
            }

            return false;
        }
    }
}
