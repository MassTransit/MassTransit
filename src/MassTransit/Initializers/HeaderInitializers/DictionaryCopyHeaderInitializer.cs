namespace MassTransit.Initializers.HeaderInitializers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


    public class DictionaryCopyHeaderInitializer<TMessage, TInput, THeader> :
        IHeaderInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class, IDictionary<string, THeader>
    {
        readonly IWriteProperty<SendContext, THeader> _headerProperty;
        readonly string _key;

        public DictionaryCopyHeaderInitializer(string headerName, string inputPropertyName = null)
        {
            if (headerName == null)
                throw new ArgumentNullException(nameof(headerName));

            _key = inputPropertyName ?? headerName;
            _headerProperty = WritePropertyCache<SendContext>.GetProperty<THeader>(headerName);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
        {
            if (context.HasInput && context.Input.TryGetValue(_key, out var value))
                _headerProperty.Set(sendContext, value);

            return TaskUtil.Completed;
        }
    }
}
