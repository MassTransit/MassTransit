namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


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

        public ProviderPropertyInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, PropertyInfo propertyInfo)
        {
            if (propertyProvider == null)
                throw new ArgumentNullException(nameof(propertyProvider));

            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            _propertyProvider = propertyProvider;

            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(propertyInfo);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            var propertyTask = _propertyProvider.GetProperty(context);
            if (propertyTask.IsCompleted)
            {
                _messageProperty.Set(context.Message, propertyTask.Result);
                return TaskUtil.Completed;
            }

            async Task ApplyAsync()
            {
                var propertyValue = await propertyTask.ConfigureAwait(false);

                _messageProperty.Set(context.Message, propertyValue);
            }

            return ApplyAsync();
        }
    }
}
