namespace MassTransit.Initializers.HeaderInitializers
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    /// <summary>
    /// Set a header to a constant value from the input
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="THeader">The header type</typeparam>
    public class CopyHeaderInitializer<TMessage, TInput, THeader> :
        IHeaderInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IWriteProperty<SendContext, THeader> _headerProperty;
        readonly IReadProperty<TInput, THeader> _inputProperty;

        public CopyHeaderInitializer(PropertyInfo headerPropertyInfo, PropertyInfo inputPropertyInfo)
        {
            if (headerPropertyInfo == null)
                throw new ArgumentNullException(nameof(headerPropertyInfo));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<THeader>(inputPropertyInfo);
            _headerProperty = WritePropertyCache<SendContext>.GetProperty<THeader>(headerPropertyInfo);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
        {
            var inputPropertyValue = _inputProperty.Get(context.Input);

            _headerProperty.Set(sendContext, inputPropertyValue);

            return Task.CompletedTask;
        }
    }
}
