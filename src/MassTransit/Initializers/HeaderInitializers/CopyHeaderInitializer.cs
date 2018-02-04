namespace MassTransit.Initializers.HeaderInitializers
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


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
        readonly IWriteProperty<SendContext<TMessage>, THeader> _headerProperty;
        readonly IReadProperty<TInput, THeader> _inputProperty;

        public CopyHeaderInitializer(string headerName, string inputPropertyName = null)
        {
            if (headerName == null)
                throw new ArgumentNullException(nameof(headerName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<THeader>(inputPropertyName ?? headerName);
            _headerProperty = WritePropertyCache<SendContext<TMessage>>.GetProperty<THeader>(headerName);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context, SendContext<TMessage> sendContext)
        {
            var inputPropertyValue = _inputProperty.Get(context.Input);

            _headerProperty.Set(sendContext, inputPropertyValue);

            return TaskUtil.Completed;
        }
    }
}