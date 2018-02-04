namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


    public class CopyObjectPropertyInitializer<TMessage, TInput, TInputProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IReadProperty<TInput, TInputProperty> _inputProperty;
        readonly IWriteProperty<TMessage, object> _messageProperty;

        public CopyObjectPropertyInitializer(string messagePropertyName, string inputPropertyName = null)
        {
            if (messagePropertyName == null)
                throw new ArgumentNullException(nameof(messagePropertyName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TInputProperty>(inputPropertyName ?? messagePropertyName);
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<object>(messagePropertyName);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            _messageProperty.Set(context.Message, _inputProperty.Get(context.Input));

            return TaskUtil.Completed;
        }
    }
}
