namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;


    public class ConvertObjectPropertyInitializer<TMessage, TInput, TProperty, TInputProperty> :
        IPropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IPropertyConverter<TProperty, TInputProperty> _converter;
        readonly IReadProperty<TInput, TInputProperty> _inputProperty;
        readonly IWriteProperty<TMessage, TProperty> _messageProperty;

        public ConvertObjectPropertyInitializer(IPropertyConverter<TProperty, TInputProperty> converter, string messagePropertyName,
            string inputPropertyName = null)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (messagePropertyName == null)
                throw new ArgumentNullException(nameof(messagePropertyName));

            _converter = converter;

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TInputProperty>(inputPropertyName ?? messagePropertyName);
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(messagePropertyName);
        }

        public async Task Apply(InitializeContext<TMessage, TInput> context)
        {
            if (context.HasInput)
            {
                var inputProperty = _inputProperty.Get(context.Input);

                var messageProperty = await _converter.Convert(context, inputProperty).ConfigureAwait(false);

                _messageProperty.Set(context.Message, messageProperty);
            }
        }
    }
}
