namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


    /// <summary>
    /// Gets the dictionary entry for the property (if present), and sets the message property to the value
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class DictionaryCopyPropertyInitializer<TMessage, TInput, TProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class, IDictionary<string, TProperty>
    {
        readonly string _key;
        readonly IWriteProperty<TMessage, TProperty> _messageProperty;

        public DictionaryCopyPropertyInitializer(string messagePropertyName, string inputPropertyName = null)
        {
            if (messagePropertyName == null)
                throw new ArgumentNullException(nameof(messagePropertyName));

            _key = inputPropertyName ?? messagePropertyName;
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(messagePropertyName);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            if (context.HasInput && context.Input.TryGetValue(_key, out var value))
                _messageProperty.Set(context.Message, value);

            return TaskUtil.Completed;
        }
    }
}
