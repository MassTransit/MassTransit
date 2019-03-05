namespace MassTransit.Initializers.HeaderInitializers
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
    public class ProviderHeaderInitializer<TMessage, TInput, TProperty> :
        IHeaderInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IPropertyProvider<TInput, TProperty> _propertyProvider;
        readonly IWriteProperty<SendContext, TProperty> _messageProperty;

        public ProviderHeaderInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, string propertyName)
        {
            if (propertyProvider == null)
                throw new ArgumentNullException(nameof(propertyProvider));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            _propertyProvider = propertyProvider;

            _messageProperty = WritePropertyCache<SendContext>.GetProperty<TProperty>(propertyName);
        }

        public async Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
        {
            var propertyValue = await _propertyProvider.GetProperty(context).ConfigureAwait(false);

            _messageProperty.Set(sendContext, propertyValue);
        }
    }
}