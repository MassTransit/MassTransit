namespace MassTransit.Configuration
{
    using System;


    public class GroupKeyProvider<TMessage, TKey> :
        IGroupKeyProvider<TMessage, TKey>
        where TMessage : class
        where TKey : class
    {
        readonly Func<ConsumeContext<TMessage>, TKey> _provider;

        public GroupKeyProvider(Func<ConsumeContext<TMessage>, TKey> provider)
        {
            _provider = provider;
        }

        public bool TryGetKey(ConsumeContext<TMessage> context, out TKey key)
        {
            key = _provider(context);

            return key != null;
        }
    }
}
