namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using PropertyProviders;


    /// <summary>
    /// Set a message property using the property provider for the property value
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class ProviderPropertyInitializer<TMessage, TInput, TProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IPropertyProvider<TInput, TProperty> _propertyProvider;
        readonly IWriteProperty<TMessage, TProperty> _messageProperty;

        public ProviderPropertyInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, string propertyName)
        {
            if (propertyProvider == null)
                throw new ArgumentNullException(nameof(propertyProvider));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            _propertyProvider = propertyProvider;

            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(propertyName);
        }

        public async Task Apply(InitializeContext<TMessage, TInput> context)
        {
            var propertyValue = await _propertyProvider.GetProperty(context).ConfigureAwait(false);

            _messageProperty.Set(context.Message, propertyValue);
        }
    }
}