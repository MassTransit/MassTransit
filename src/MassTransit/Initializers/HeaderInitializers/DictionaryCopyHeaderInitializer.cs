namespace MassTransit.Initializers.HeaderInitializers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;


    public class DictionaryCopyHeaderInitializer<TMessage, TInput, THeader> :
        IHeaderInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class, IDictionary<string, THeader>
    {
        readonly IWriteProperty<SendContext, THeader> _headerProperty;
        readonly string _key;

        public DictionaryCopyHeaderInitializer(PropertyInfo propertyInfo, string key)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            _key = key;
            _headerProperty = WritePropertyCache<SendContext>.GetProperty<THeader>(propertyInfo);
        }

        public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
        {
            if (context.HasInput && context.Input.TryGetValue(_key, out var value))
                _headerProperty.Set(sendContext, value);

            return Task.CompletedTask;
        }
    }
}
