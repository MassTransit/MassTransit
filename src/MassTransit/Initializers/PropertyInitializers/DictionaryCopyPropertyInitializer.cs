namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


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

        public DictionaryCopyPropertyInitializer(PropertyInfo propertyInfo, string key)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            _key = key;
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(propertyInfo);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context)
        {
            if (context.HasInput && context.Input.TryGetValue(_key, out var value))
                _messageProperty.Set(context.Message, value);

            return Task.CompletedTask;
        }
    }
}
