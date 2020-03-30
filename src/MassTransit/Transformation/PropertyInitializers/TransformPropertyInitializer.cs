namespace MassTransit.Transformation.PropertyInitializers
{
    using System;
    using System.Threading.Tasks;
    using Initializers;


    /// <summary>
    /// Set a message property using the property provider for the property value
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class TransformPropertyInitializer<TMessage, TInput, TProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IPropertyProvider<TInput, TProperty> _propertyProvider;

        public TransformPropertyInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, System.Reflection.PropertyInfo propertyInfo)
        {
            if (propertyProvider == null)
                throw new ArgumentNullException(nameof(propertyProvider));

            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            _propertyProvider = propertyProvider;
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            return _propertyProvider.GetProperty(context);
        }
    }
}
