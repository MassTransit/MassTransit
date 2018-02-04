namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


    /// <summary>
    /// Set a message property by copying the input property (of the same type), regardless of whether
    /// the input property value is null, etc.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class CopyPropertyInitializer<TMessage, TInput, TProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IReadProperty<TInput, TProperty> _inputProperty;
        readonly IWriteProperty<TMessage, TProperty> _messageProperty;

        public CopyPropertyInitializer(string messagePropertyName, string inputPropertyName = null)
        {
            if (messagePropertyName == null)
                throw new ArgumentNullException(nameof(messagePropertyName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TProperty>(inputPropertyName ?? messagePropertyName);
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(messagePropertyName);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            if (context.HasInput)
            {
                _messageProperty.Set(context.Message, _inputProperty.Get(context.Input));
            }

            return TaskUtil.Completed;
        }
    }
}
