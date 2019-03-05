namespace MassTransit.Initializers.Factories
{
    using Conventions;


    public class InputHeaderInitializerInspector<TMessage, TInput> :
        IHeaderInitializerInspector<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly string _propertyName;

        public InputHeaderInitializerInspector(string propertyName)
        {
            _propertyName = propertyName;
        }

        public bool Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention)
        {
            if (builder.IsInputPropertyUsed(_propertyName))
                return false;

            if (convention.TryGetHeaderInitializer(_propertyName, out IHeaderInitializer<TMessage, TInput> initializer))
            {
                builder.Add(initializer);

                builder.SetInputPropertyUsed(_propertyName);
                return true;
            }

            return false;
        }
    }
}