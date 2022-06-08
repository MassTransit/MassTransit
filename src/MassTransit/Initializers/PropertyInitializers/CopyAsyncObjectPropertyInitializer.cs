namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    public class CopyAsyncObjectPropertyInitializer<TMessage, TInput, TInputProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IReadProperty<TInput, Task<TInputProperty>> _inputProperty;
        readonly IWriteProperty<TMessage, object> _messageProperty;

        public CopyAsyncObjectPropertyInitializer(PropertyInfo messagePropertyInfo, PropertyInfo inputPropertyInfo)
        {
            if (messagePropertyInfo == null)
                throw new ArgumentNullException(nameof(messagePropertyInfo));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<Task<TInputProperty>>(inputPropertyInfo);
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<object>(messagePropertyInfo);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            if (!context.HasInput)
                return Task.CompletedTask;

            Task<TInputProperty> valueTask = _inputProperty.Get(context.Input);
            if (valueTask.Status == TaskStatus.RanToCompletion)
            {
                _messageProperty.Set(context.Message, valueTask.Result);

                return Task.CompletedTask;
            }

            async Task SetPropertyAsync()
            {
                var value = await valueTask.ConfigureAwait(false);

                _messageProperty.Set(context.Message, value);
            }

            return SetPropertyAsync();
        }
    }
}
