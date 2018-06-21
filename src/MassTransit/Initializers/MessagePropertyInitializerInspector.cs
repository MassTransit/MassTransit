namespace MassTransit.Initializers
{
    using Conventions;


    public class MessagePropertyInitializerInspector<TMessage, TInput, TProperty> :
        IMessagePropertyInitializerInspector<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly string _propertyName;

        public MessagePropertyInitializerInspector(string propertyName)
        {
            _propertyName = propertyName;
        }

        public void Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention)
        {
            if (convention.TryGetMessagePropertyInitializer<TMessage, TInput, TProperty>(_propertyName, out var initializer))
                builder.Add(_propertyName, initializer);
        }
    }
}