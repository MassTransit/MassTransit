namespace MassTransit.Initializers.HeaderInitializers
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


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
        readonly IWriteProperty<SendContext, TProperty> _messageProperty;
        readonly IPropertyProvider<TInput, TProperty> _propertyProvider;

        public ProviderHeaderInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, PropertyInfo propertyInfo)
        {
            if (propertyProvider == null)
                throw new ArgumentNullException(nameof(propertyProvider));

            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            _propertyProvider = propertyProvider;

            _messageProperty = WritePropertyCache<SendContext>.GetProperty<TProperty>(propertyInfo);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
        {
            Task<TProperty> propertyTask = _propertyProvider.GetProperty(context);
            if (propertyTask.IsCompleted)
            {
                _messageProperty.Set(sendContext, propertyTask.Result);
                return Task.CompletedTask;
            }

            return ApplyAsync(sendContext, propertyTask);
        }

        async Task ApplyAsync(SendContext sendContext, Task<TProperty> propertyTask)
        {
            var propertyValue = await propertyTask.ConfigureAwait(false);

            _messageProperty.Set(sendContext, propertyValue);
        }
    }
}
