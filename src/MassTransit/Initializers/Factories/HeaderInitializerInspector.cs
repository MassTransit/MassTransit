namespace MassTransit.Initializers.Factories
{
    using Conventions;


    public class HeaderInitializerInspector<TMessage, TInput, TProperty> :
        IHeaderInitializerInspector<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly string _propertyName;

        public HeaderInitializerInspector(string propertyName)
        {
            _propertyName = propertyName;
        }

        public bool Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention)
        {
            if (builder.IsInputPropertyUsed(_propertyName))
                return false;

            if (convention.TryGetHeaderInitializer<TMessage, TInput, TProperty>(_propertyName, out IHeaderInitializer<TMessage, TInput> initializer))
            {
                builder.Add(initializer);

                builder.SetInputPropertyUsed(_propertyName);
                return true;
            }

            return false;
        }
    }
}