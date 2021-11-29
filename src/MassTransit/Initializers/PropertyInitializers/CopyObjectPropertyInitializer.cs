namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    public class CopyObjectPropertyInitializer<TMessage, TInput, TInputProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IReadProperty<TInput, TInputProperty> _inputProperty;
        readonly IWriteProperty<TMessage, object> _messageProperty;

        public CopyObjectPropertyInitializer(PropertyInfo messagePropertyInfo, PropertyInfo inputPropertyInfo)
        {
            if (messagePropertyInfo == null)
                throw new ArgumentNullException(nameof(messagePropertyInfo));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TInputProperty>(inputPropertyInfo);
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<object>(messagePropertyInfo);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            _messageProperty.Set(context.Message, _inputProperty.Get(context.Input));

            return Task.CompletedTask;
        }
    }
}
