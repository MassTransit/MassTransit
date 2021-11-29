namespace MassTransit.Configuration
{
    using System;


    public class ValueTypeGroupKeyProvider<TMessage, TKey> :
        IGroupKeyProvider<TMessage, TKey>
        where TMessage : class
        where TKey : struct
    {
        readonly Func<ConsumeContext<TMessage>, TKey?> _provider;

        public ValueTypeGroupKeyProvider(Func<ConsumeContext<TMessage>, TKey?> provider)
        {
            _provider = provider;
        }

        public bool TryGetKey(ConsumeContext<TMessage> context, out TKey key)
        {
            var property = _provider(context);

            if (property.HasValue)
            {
                key = property.Value;
                return true;
            }

            key = default;
            return false;
        }
    }
}
