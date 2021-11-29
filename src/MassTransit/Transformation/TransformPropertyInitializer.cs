namespace MassTransit.Transformation
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Initializers;
    using Internals;


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
        readonly IWriteProperty<TMessage, TProperty> _messageProperty;
        readonly IPropertyProvider<TInput, TProperty> _propertyProvider;

        public TransformPropertyInitializer(IPropertyProvider<TInput, TProperty> propertyProvider, PropertyInfo propertyInfo)
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
            Task<TProperty> propertyTask = _propertyProvider.GetProperty(context);
            if (propertyTask.IsCompleted)
            {
                if (_messageProperty.TargetType == context.MessageType)
                    _messageProperty.Set(context.Message, propertyTask.Result);
                return Task.CompletedTask;
            }

            async Task ApplyAsync()
            {
                var propertyValue = await propertyTask.ConfigureAwait(false);

                if (_messageProperty.TargetType == context.MessageType)
                    _messageProperty.Set(context.Message, propertyValue);
            }

            return ApplyAsync();
        }
    }
}
